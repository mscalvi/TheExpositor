using BingoCreator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BingoCreator.Services
{
    internal class ImportingService
    {
        public static ErrorModel ImportImages (List<string> images, string listName)
        {
            string coverFile = images
                .FirstOrDefault(f =>
                    Path.GetFileNameWithoutExtension(f)
                        .Equals(".Capa", StringComparison.OrdinalIgnoreCase)
                );

            string coverImageName = coverFile != null
                ? Path.GetFileName(coverFile)
                : null;

            int listId;
            try
            {
                listId = DataService.CreateList(listName, description: "", imagename: coverImageName);

                var seenBaseNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var file in images)
                {
                    string fileNameNoExt = Path.GetFileNameWithoutExtension(file);
                    if (fileNameNoExt.Equals("Capa", StringComparison.OrdinalIgnoreCase))
                        continue;

                    string baseName = fileNameNoExt?.Trim() ?? "";

                    if (string.IsNullOrWhiteSpace(baseName))
                    {
                        return new ErrorModel
                        {
                            Success = false,
                            ListError = ErrorModel.CreateListError.MissingRequired,
                            Message = "Elemento com Nome Inválido."
                        };
                    }

                    if (!seenBaseNames.Add(baseName))
                    {
                        return new ErrorModel
                        {
                            Success = false,
                            ListError = ErrorModel.CreateListError.MissingRequired,
                            Message = "Elemento Duplicado."
                        };
                    }

                    int elementId = 0;
                    try
                    {
                        elementId = DataService.CreateElement(
                            name: $"{listName} - {baseName}",
                            cardName: baseName,
                            note1: "",
                            note2: "",
                            imageName: Path.GetFileName(file),
                            addTime: DateTime.Now.ToString("MMddyyyy - HH:mm:ss")
                        );

                        if (elementId <= 0)
                        {
                            return new ErrorModel
                            {
                                Success = false,
                                ListError = ErrorModel.CreateListError.MissingRequired,
                                Message = "Elemento Não Criado."
                            };
                        }
                    }
                    catch (Exception exCreate)
                    {
                        return new ErrorModel
                        {
                            Success = false,
                            ListError = ErrorModel.CreateListError.MissingRequired,
                            Message = "Elemento Não Criado."
                        };
                    }

                    try
                    {
                        DataService.AlocateElements(listId, new List<int> { elementId });
                    }
                    catch (Exception exLink)
                    {
                        return new ErrorModel
                        {
                            Success = false,
                            ListError = ErrorModel.CreateListError.MissingRequired,
                            Message = "Elementos não Alocados."
                        };
                    }
                }

                return new ErrorModel
                {
                    Success = true,
                    Message = "Lista criada com sucesso."
                };
            }
            catch (Exception ex)
            {
                return new ErrorModel
                {
                    Success = false,
                    ListError = ErrorModel.CreateListError.DbFailure,
                    Message = "Erro ao acessar o DB."
                };
            }
        }

        public static ErrorModel ImportTxt (string filePath, string listName)
        {
            string text;

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var aprovados = new List<string>();
            var rejeitados = new List<string>();

            try
            {
                text = File.ReadAllText(filePath, Encoding.UTF8);
            }
            catch (DecoderFallbackException)
            {
                text = File.ReadAllText(filePath, Encoding.GetEncoding(1252));
            }

            var rawTokens = text
                .Replace("\r\n", "\n").Replace("\r", "\n")
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .SelectMany(l => l.Split(new[] { ',', ';', '\t' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                .Select(t => t.Trim());

            try
            {
                foreach (var raw in rawTokens)
                {
                    var name = raw.Trim();

                    if (string.IsNullOrWhiteSpace(name))
                        continue;

                    if (name.Length > 50)
                    {
                        rejeitados.Add($"{raw}  — > 50 caracteres");
                        continue;
                    }

                    if (!seen.Add(name))
                        continue;

                    aprovados.Add(name);
                }

                int listId = DataService.CreateList(listName, description: "", imagename: null);

                foreach (var name in aprovados)
                {
                    int elementId = 0;
                    try
                    {
                        elementId = DataService.CreateElement(
                            name: $"{listName} - {name}",
                            cardName: name,
                            note1: "",
                            note2: "",
                            imageName: null,
                            addTime: DateTime.Now.ToString("MMddyyyy - HH:mm:ss")
                        );
                    }
                    catch
                    {
                        return new ErrorModel
                        {
                            Success = false,
                            ListError = ErrorModel.CreateListError.MissingRequired,
                            Message = "Elementos não criados."
                        };
                    }

                    try
                    {
                        DataService.AlocateElements(listId, new List<int> { elementId });
                    }
                    catch
                    {
                        return new ErrorModel
                        {
                            Success = false,
                            ListError = ErrorModel.CreateListError.MissingRequired,
                            Message = "Elementos não Alocados."
                        };
                    }
                }
                return new ErrorModel
                {
                    Success = true,
                    Message = "Sucesso em importar a Lista."
                };
            }
            catch (Exception ex)
            {
                return new ErrorModel
                {
                    Success = false,
                    ListError = ErrorModel.CreateListError.DbFailure,
                    Message = "Falha ao acessar o DB."
                };
            }
        }
    }
}
