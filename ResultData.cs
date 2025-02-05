
using System.Runtime.Serialization;

namespace Plugfy.Core.Commons
{
    [Serializable]
    public class ResultData
    {

        [DataMember(Name = "IDName", IsRequired = true)]
        public string? IDName { get; set; } 

        [DataMember(Name = "Value", IsRequired = false)]
        public dynamic? Value { get; set; }

    }
}
