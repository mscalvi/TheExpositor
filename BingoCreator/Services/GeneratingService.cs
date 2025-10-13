using BingoCreator.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace BingoCreator.Services
{
    internal class GeneratingService
    {
        private static readonly Random _rng = new Random();
        public static int CreateCards(CardSetModel cards)
        {
            Random random = new Random();

            List<List<DataRow>> allCards = new List<List<DataRow>>();

            List<DataRow> ElementsList = DataService.GetElementsInList(cards.ListId);

            ElementsList = ElementsList.OrderBy(x => random.Next()).ToList();

            cards.AllElements = ElementsList.Select(ToElementModel).ToList();

            int elementsPerColumn = 1;
            int remainder = 1;

            cards.AddDate = DateTime.Now.ToString("MMddyyyy - HH:mm:ss");

            if (cards.CardsSize == 5)
            {
                elementsPerColumn = ElementsList.Count / 5;
                remainder = ElementsList.Count % 5;

                List<DataRow> columnB = ElementsList.Take(elementsPerColumn + (remainder > 0 ? 1 : 0)).ToList();
                List<DataRow> columnI = ElementsList.Skip(columnB.Count).Take(elementsPerColumn + (remainder > 1 ? 1 : 0)).ToList();
                List<DataRow> columnN = ElementsList.Skip(columnB.Count + columnI.Count).Take(elementsPerColumn + (remainder > 2 ? 1 : 0)).ToList();
                List<DataRow> columnG = ElementsList.Skip(columnB.Count + columnI.Count + columnN.Count).Take(elementsPerColumn + (remainder > 3 ? 1 : 0)).ToList();
                List<DataRow> columnO = ElementsList.Skip(columnB.Count + columnI.Count + columnN.Count + columnG.Count).Take(elementsPerColumn).ToList();

                cards.GroupB = columnB.Select(ToElementModel).ToList();
                cards.GroupI = columnI.Select(ToElementModel).ToList();
                cards.GroupN = columnN.Select(ToElementModel).ToList();
                cards.GroupG = columnG.Select(ToElementModel).ToList();
                cards.GroupO = columnO.Select(ToElementModel).ToList();

                cards.GroupBIds = string.Join(",", columnB.Select(c => c["Id"].ToString()));
                cards.GroupIIds = string.Join(",", columnI.Select(c => c["Id"].ToString()));
                cards.GroupNIds = string.Join(",", columnN.Select(c => c["Id"].ToString()));
                cards.GroupGIds = string.Join(",", columnG.Select(c => c["Id"].ToString()));
                cards.GroupOIds = string.Join(",", columnO.Select(c => c["Id"].ToString()));

                int setId = DataService.CreateCardsSet(cards);

                for (int i = 1; i <= cards.Quantity; i++)
                {
                    var tempB = new List<DataRow>(columnB);
                    var tempI = new List<DataRow>(columnI);
                    var tempN = new List<DataRow>(columnN);
                    var tempG = new List<DataRow>(columnG);
                    var tempO = new List<DataRow>(columnO);
                    var selected = new List<DataRow>();

                    selected.AddRange(SelectAndRemoveFromGroup(tempB, 5, random));
                    selected.AddRange(SelectAndRemoveFromGroup(tempI, 5, random));
                    selected.AddRange(SelectAndRemoveFromGroup(tempN, 5, random));
                    selected.AddRange(SelectAndRemoveFromGroup(tempG, 5, random));
                    selected.AddRange(SelectAndRemoveFromGroup(tempO, 5, random));

                    var companyIds = selected.Select(c => Convert.ToInt32(c["Id"])).ToList();
                    if (companyIds.Count == 25)
                    {
                        DataService.CreateCard5(cards.ListId, companyIds, i, setId);
                        allCards.Add(selected);
                    }
                }
                return setId;

            } else if (cards.CardsSize == 4)
            {
                int setId = DataService.CreateCardsSet(cards);

                for (int i = 1; i <= cards.Quantity; i++)
                {
                    var tempList = new List<DataRow>(ElementsList);

                    var selected = SelectAndRemoveFromGroup(tempList, 16, random);

                    var elementIds = selected
                        .Select(c => Convert.ToInt32(c["Id"]))
                        .ToList();

                    if (elementIds.Count == 16)
                    {
                        DataService.CreateCard4(cards.ListId, elementIds, i, setId);
                        allCards.Add(selected);
                    }
                }

                return setId;
            } else
            {
                return -1;
            }
        }

        private static List<DataRow> SelectAndRemoveFromGroup(List<DataRow> group, int count, Random random)
        {
            var selected = new List<DataRow>();
            for (int i = 0; i < count && group.Count > 0; i++)
            {
                int idx = random.Next(group.Count);
                selected.Add(group[idx]);
                group.RemoveAt(idx);
            }
            return selected;
        }

        private static ElementModel ToElementModel(DataRow r) => new ElementModel
        {
            Id = Convert.ToInt32(r["Id"]),
            Name = r["Name"]?.ToString() ?? "",
            CardName = r["CardName"]?.ToString() ?? "",
            ImageName = r["ImageName"]?.ToString() ?? ""
        };


        public static int AddCardsToSet(int setId, int howMany)
        {
            if (setId <= 0 || howMany <= 0) return 0;

            // 1) Carrega metadados do set + pools
            var set = DataService.GetCardSetById(setId);
            if (set == null) throw new InvalidOperationException("Set não encontrado.");

            int created = 0;
            int nextNumber = DataService.GetMaxCardNumberBySetId(setId) + 1;

            if (set.CardsSize == 5)
            {
                // pools por coluna (IDs)
                var poolB = (set.GroupB ?? new List<ElementModel>()).Select(e => e.Id).Distinct().ToList();
                var poolI = (set.GroupI ?? new List<ElementModel>()).Select(e => e.Id).Distinct().ToList();
                var poolN = (set.GroupN ?? new List<ElementModel>()).Select(e => e.Id).Distinct().ToList();
                var poolG = (set.GroupG ?? new List<ElementModel>()).Select(e => e.Id).Distinct().ToList();
                var poolO = (set.GroupO ?? new List<ElementModel>()).Select(e => e.Id).Distinct().ToList();

                // sanidade
                if (poolB.Count < 5 || poolI.Count < 5 || poolN.Count < 5 || poolG.Count < 5 || poolO.Count < 5)
                    throw new InvalidOperationException("Alguma coluna possui menos de 5 elementos disponíveis.");

                var signatures = DataService.GetExistingSignatures5(setId);

                // 2) gera novas cartelas
                for (int k = 0; k < howMany; k++)
                {
                    // tenta algumas permutações diferentes até não colidir
                    const int MAX_TRIES = 200;
                    List<int> chosenB = null, chosenI = null, chosenN = null, chosenG = null, chosenO = null;
                    string sig = null;

                    for (int t = 0; t < MAX_TRIES; t++)
                    {
                        chosenB = TakeRandom(poolB, 5);
                        chosenI = TakeRandom(poolI, 5);
                        chosenN = TakeRandom(poolN, 5);
                        chosenG = TakeRandom(poolG, 5);
                        chosenO = TakeRandom(poolO, 5);

                        var all = new List<int>(25);
                        all.AddRange(chosenB);
                        all.AddRange(chosenI);
                        all.AddRange(chosenN);
                        all.AddRange(chosenG);
                        all.AddRange(chosenO);

                        sig = string.Join("-", all);
                        if (!signatures.Contains(sig))
                        {
                            signatures.Add(sig);
                            break;
                        }
                        sig = null;
                    }

                    if (sig == null)
                        throw new InvalidOperationException("Não foi possível gerar cartelas novas sem duplicar as já existentes.");

                    // monta na ordem esperada por CreateCard5 (B... O...)
                    var elementsIds = new List<int>(25);
                    elementsIds.AddRange(chosenB);
                    elementsIds.AddRange(chosenI);
                    elementsIds.AddRange(chosenN);
                    elementsIds.AddRange(chosenG);
                    elementsIds.AddRange(chosenO);

                    DataService.CreateCard5(set.ListId, elementsIds, nextNumber++, setId);
                    created++;
                }
            }
            else if (set.CardsSize == 4)
            {
                // pool global de IDs (TODOS os elementos do set, não apenas 16)
                var pool = (set.AllElements ?? new List<ElementModel>()).Select(e => e.Id).Distinct().ToList();
                if (pool.Count < 16)
                    throw new InvalidOperationException("O conjunto 4×4 precisa ter pelo menos 16 elementos disponíveis.");

                var signatures = DataService.GetExistingSignatures4(setId);

                for (int k = 0; k < howMany; k++)
                {
                    const int MAX_TRIES = 200;
                    List<int> chosen = null;
                    string sig = null;

                    for (int t = 0; t < MAX_TRIES; t++)
                    {
                        chosen = TakeRandom(pool, 16);
                        sig = string.Join("-", chosen);
                        if (!signatures.Contains(sig))
                        {
                            signatures.Add(sig);
                            break;
                        }
                        sig = null;
                    }

                    if (sig == null)
                        throw new InvalidOperationException("Não foi possível gerar cartelas novas sem duplicar as já existentes.");

                    DataService.CreateCard4(set.ListId, chosen, nextNumber++, setId);
                    created++;
                }
            }
            else
            {
                throw new InvalidOperationException($"CardsSize inválido ({set.CardsSize}).");
            }

            // 3) Atualiza Quantity do set
            DataService.UpdateCardSetQuantity(setId, (set.Quantity) + created);

            return created;
        }

        // utilitário: sorteia k itens distintos da lista (sem modificar a original)
        private static List<int> TakeRandom(List<int> source, int k)
        {
            var arr = source.ToList(); // cópia
                                       // Fisher–Yates parcial
            for (int i = 0; i < k; i++)
            {
                int j = _rng.Next(i, arr.Count);
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }
            return arr.Take(k).ToList();
        }
    }
}
