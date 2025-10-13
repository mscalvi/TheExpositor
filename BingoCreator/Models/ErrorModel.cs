using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingoCreator.Models
{
    internal class ErrorModel
    {
        public bool Success { get; init; }
        public int Id { get; init; }
        public CreateElementError? EleError { get; init; }
        public CreateListError? ListError { get; init; }
        public CreateCardsError? CardError { get; init; }
        public string Message { get; init; } = "";
        public enum CreateElementError
        {
            None,
            MissingRequired,
            NameTooLong,
            CardNameTooLong,
            NoteTooLong,
            DuplicateName,
            InvalidList,
            DbFailure
        }
        public enum CreateListError
        {
            None,
            MissingRequired,
            NameTooLong,
            NoteTooLong,
            DuplicateName,
            DbFailure
        }
        public enum CreateCardsError
        {
            None,
            MissingName,
            NameTooLong,
            MissingTitle,
            TitleTooLong,
            MissingEnd,
            EndTooLong,
            InvalidList,
            DuplicateName,
            DbFailure
        }
        
    }
}
