using System.Linq;
using ContaJunsta.Models;

namespace ContaJunsta.Services;

public class CalculationService
{
    public record PersonLite(string Id, string Name);
    public record PersonSummary(string Id, string Name, int Paid, int Should, int Balance);
    public record Transfer(string FromId, string ToId, int Cents);
    public record CalcResult(List<PersonSummary> Summaries, List<Transfer> Transfers);

    public CalcResult Compute(IReadOnlyList<PersonLite> persons, IReadOnlyList<BillModel> bills)
    {
        var people = persons.ToList();
        var byId = people.ToDictionary(p => p.Id);

        var paidExpenses = people.ToDictionary(p => p.Id, _ => 0);
        var shouldExpenses = people.ToDictionary(p => p.Id, _ => 0);
        var shareGains = people.ToDictionary(p => p.Id, _ => 0);
        var shouldPayerGains = people.ToDictionary(p => p.Id, _ => 0);

        int bucketExpense = 0, bucketGain = 0;
        var active = new HashSet<string>();

        foreach (var b in bills ?? Array.Empty<BillModel>())
        {
            var sel = (b.ParticipantIds ?? new List<string>()).Where(byId.ContainsKey).ToList();
            if (sel.Count == 0) continue;

            var abs = Math.Abs(b.Cents);
            var q = abs / sel.Count;
            var r = abs % sel.Count;

            if (!string.IsNullOrEmpty(b.ResponsiblePersonId)) active.Add(b.ResponsiblePersonId);
            foreach (var s in sel) active.Add(s);

            if (b.Cents > 0)
            {
                if (byId.ContainsKey(b.ResponsiblePersonId ?? "")) paidExpenses[b.ResponsiblePersonId!] += abs;
                foreach (var s in sel) shouldExpenses[s] += q;
                bucketExpense += r;
            }
            else if (b.Cents < 0)
            {
                if (byId.ContainsKey(b.ResponsiblePersonId ?? "")) shouldPayerGains[b.ResponsiblePersonId!] += abs;
                foreach (var s in sel) shareGains[s] += q;
                bucketGain += r;
            }
        }

        var order = people
            .Where(p => active.Contains(p.Id))
            .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
            .Select(p => p.Id)
            .ToList();

        var expenseAdj = people.ToDictionary(p => p.Id, _ => 0);
        var gainAdj = people.ToDictionary(p => p.Id, _ => 0);

        if (order.Count > 0)
        {
            for (int i = 0; i < bucketExpense; i++) expenseAdj[order[i % order.Count]] += 1;
            for (int i = 0; i < bucketGain; i++) gainAdj[order[i % order.Count]] += 1;
        }

        var summaries = new List<PersonSummary>(people.Count);
        foreach (var p in people)
        {
            var should = (shouldExpenses[p.Id] + expenseAdj[p.Id])
                       - (shareGains[p.Id] + gainAdj[p.Id])
                       + shouldPayerGains[p.Id];

            var paid = paidExpenses[p.Id];
            var balance = paid - should;
            summaries.Add(new PersonSummary(p.Id, p.Name, paid, should, balance));
        }

        var creditors = summaries.Where(s => s.Balance > 0).Select(s => (s.Id, s.Balance)).OrderByDescending(x => x.Balance).ToList();
        var debtors = summaries.Where(s => s.Balance < 0).Select(s => (s.Id, -s.Balance)).OrderByDescending(x => x.Item2).ToList();

        var transfers = new List<Transfer>();
        int ci = 0, di = 0;
        while (ci < creditors.Count && di < debtors.Count)
        {
            var take = Math.Min(creditors[ci].Balance, debtors[di].Item2);
            if (take > 0)
            {
                transfers.Add(new Transfer(debtors[di].Id, creditors[ci].Id, take));
                creditors[ci] = (creditors[ci].Id, creditors[ci].Balance - take);
                debtors[di] = (debtors[di].Id, debtors[di].Item2 - take);
            }
            if (creditors[ci].Balance == 0) ci++;
            if (debtors[di].Item2 == 0) di++;
        }

        return new CalcResult(summaries, transfers);
    }
}
