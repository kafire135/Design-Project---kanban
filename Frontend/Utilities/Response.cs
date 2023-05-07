using System;
using System.Text.Json.Serialization;

#nullable enable
namespace IntroSE.Kanban.Frontend.Utilities
{
    [Serializable]
    public sealed class Response<T>
    {
        [JsonInclude]
        public readonly bool operationState;

        [JsonInclude]
        public readonly T returnValue;

        [JsonConstructor]
        public Response(bool operationState, T returnValue)
        {
            this.operationState = operationState;
            this.returnValue = returnValue;
        }
    }


}
