using BingoCreator.Models;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace BingoCreator.Services
{
    internal class CreatingService
    {
        public static ErrorModel CreateElement(ElementModel element, ListModel list)
        {
            int maxNameLength = 100;
            int maxCardNameLength = 60;
            int maxNotesLength = 250;

            element.Note1 ??= "";
            element.Note2 ??= "";

            if (string.IsNullOrEmpty(element.Name) || string.IsNullOrEmpty(element.CardName))
            {
                return new ErrorModel
                {
                    Success = false,
                    EleError = ErrorModel.CreateElementError.MissingRequired,
                    Message = "Nome e Nome para Cartela são obrigatórios."
                };
            }

            if (element.Name.Length > maxNameLength)
            {
                return new ErrorModel
                {
                    Success = false,
                    EleError = ErrorModel.CreateElementError.NameTooLong,
                    Message = $"Nome do Elemento deve ter no máximo {maxNameLength} caracteres."
                };
            }

            if (element.CardName.Length > maxCardNameLength)
            {
                return new ErrorModel
                {
                    Success = false,
                    EleError = ErrorModel.CreateElementError.CardNameTooLong,
                    Message = $"O Nome para Cartela deve ter no máximo {maxCardNameLength} caracteres."
                };
            }

            if (element.Note1.Length > maxNotesLength)
            {
                return new ErrorModel
                {
                    Success = false,
                    EleError = ErrorModel.CreateElementError.NoteTooLong,
                    Message = $"A anotação 1 deve ter no máximo {maxNotesLength} caracteres."
                };
            }

            if (element.Note2.Length > maxNotesLength)
            {
                return new ErrorModel
                {
                    Success = false,
                    EleError = ErrorModel.CreateElementError.NoteTooLong,
                    Message = $"A anotação 2 deve ter no máximo {maxNotesLength} caracteres."
                };
            }

            try
            {
                string relativePath = Path.Combine("images", ".nolist", element.CardName + ".png");
                element.AddTime = DateTime.Now.ToString("MMddyyyy - HH:mm:ss");
                element.Id = DataService.CreateElement(element.Name, element.CardName, element.Note1, element.Note2, relativePath, element.AddTime);

                if (list.Id > 0)
                {
                    List<int> elements = new List<int>();
                    elements.Add(element.Id);

                    try
                    {
                        DataService.AlocateElements(list.Id, elements);

                        return new ErrorModel
                        {
                            Success = true,
                            Message = $"Elemento {element.Name} adicionado à Lista {list.Name} com sucesso."
                        };
                    }
                    catch (Exception ex)
                    {
                        return new ErrorModel
                        {
                            Success = false,
                            EleError = ErrorModel.CreateElementError.InvalidList,
                            Message = $"Falha ao adicionar Elemento {element.Name} à Lista {list.Name}."
                        };
                    }
                }
                else
                {
                    return new ErrorModel
                    {
                        Success = true,
                        Id = element.Id,
                        Message = $"Elemento {element.Name} criado com sucesso."
                    };
                }
            }
            catch (Exception ex)
            {
                return new ErrorModel
                {
                    Success = false,
                    EleError = ErrorModel.CreateElementError.DbFailure,
                    Message = $"Falha ao criar Elemento {element.Name}."
                };
            }
        }

        public static ErrorModel CreateList(ListModel list)
        {
            int maxNameLength = 100;
            int maxDescriptionLength = 300;

            if (string.IsNullOrEmpty(list.Description))
            {
                list.Description = "*";
            }

            if (string.IsNullOrEmpty(list.Name))
            {
                return new ErrorModel
                {
                    Success = false,
                    ListError = ErrorModel.CreateListError.MissingRequired,
                    Message = "Nome da lista não pode estar em branco."
                };
            }

            if (list.Name.Length > maxNameLength)
            {
                return new ErrorModel
                {
                    Success = false,
                    ListError = ErrorModel.CreateListError.NameTooLong,
                    Message = $"O nome da Lista deve ter no máximo {maxNameLength} caracteres."
                };
            }

            if (list.Description.Length > maxDescriptionLength)
            {
                return new ErrorModel
                {
                    Success = false,
                    ListError = ErrorModel.CreateListError.NoteTooLong,
                    Message = $"A descrição da Lista deve ter no máximo {maxDescriptionLength} caracteres."
                };
            }

            try
            {
                string relativePath = Path.Combine("images", "Capa.png");
                list.Id = DataService.CreateList(list.Name, list.Description, relativePath);

                return new ErrorModel
                {
                    Success = true,
                    Id = list.Id,
                    Message = $"Lista {list.Name} criada com sucesso."
                };
            }
            catch (Exception ex)
            {
                return new ErrorModel
                {
                    Success = false,
                    ListError = ErrorModel.CreateListError.DbFailure,
                    Message = $"Falha ao criar a lista {list.Name}."
                };
            }
        }

        public static ErrorModel CreateCards(CardSetModel cards)
        {
            int maxNameLength = 50;
            int maxTitleLength = 120;
            int maxEndLength = 200;
            int minElements = 0;

            if(cards.CardsSize == 4)
            {
                minElements = 40;
            } else
            {
                minElements = 50;
            }

            ListModel list = DataService.GetListById(cards.ListId);
            cards.ListName = list.Name;
            cards.ListSize = list.ElementCount;
            cards.ImageName = list.ImageName;

            if (string.IsNullOrEmpty(cards.Name))
            {
                return new ErrorModel
                {
                    Success = false,
                    CardError = ErrorModel.CreateCardsError.MissingName,
                    Message = "O nome do conjunto é obrigatório!"
                };
            }

            if (cards.Name.Length > maxNameLength)
            {
                return new ErrorModel
                {
                    Success = false,
                    CardError = ErrorModel.CreateCardsError.NameTooLong,
                    Message = $"O nome do conunto deve ter no máximo {maxNameLength} caracteres!"
                };
            }

            if (string.IsNullOrEmpty(cards.Title))
            {
                return new ErrorModel
                {
                    Success = false,
                    CardError = ErrorModel.CreateCardsError.MissingTitle,
                    Message = "O título da cartela é obrigatório!"
                };
            }

            if (cards.Title.Length > maxTitleLength)
            {
                return new ErrorModel
                {
                    Success = false,
                    CardError = ErrorModel.CreateCardsError.TitleTooLong,
                    Message = $"O título da cartela deve ter no máximo {maxTitleLength} caracteres!"
                };
            }

            if (string.IsNullOrEmpty(cards.End))
            {
                return new ErrorModel
                {
                    Success = false,
                    CardError = ErrorModel.CreateCardsError.MissingEnd,
                    Message = "A mensagem final da cartela é obrigatória!"
                };
            }

            if (cards.End.Length > maxTitleLength)
            {
                return new ErrorModel
                {
                    Success = false,
                    CardError = ErrorModel.CreateCardsError.EndTooLong,
                    Message = $"A mensagem final da cartela deve ter no máximo {maxEndLength} caracteres!"
                };
            }

            if (cards.ListSize < minElements)
            {
                return new ErrorModel
                {
                    Success = false,
                    CardError = ErrorModel.CreateCardsError.InvalidList,
                    Message = $"A lista {cards.ListName} tem apenas {cards.ListSize} Elementos, o mínimo é de {minElements}."
                };
            }

            try
            {
                cards.Id = GeneratingService.CreateCards(cards);

                return new ErrorModel
                {
                    Success = true,
                    Id = cards.Id,
                    Message = "Cartelas adicionadas ao Bando de Dados com sucesso."
                };
            }
            catch (Exception ex)
            {
                return new ErrorModel
                {
                    Success = false,
                    CardError = ErrorModel.CreateCardsError.DbFailure,
                    Message = $"Falha ao criar as cartelas {cards.Name}."
                };
            }
        }
    }
}
