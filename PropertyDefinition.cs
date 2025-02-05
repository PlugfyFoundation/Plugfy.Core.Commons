using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Plugfy.Core.Commons
{
    public class PropertyDefinition
    {
        public string? GroupName { get; set; }
        public required string IDName { get; set; }
        public enumDefaultUIValueType ParameterType { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool Mandatory { get; set; }
        public dynamic? DefaultValue { get; set; }
        public dynamic? Value { get; set; }
        public int? Order { get; set; }
        public bool HiddenField { get; set; }

        [JsonIgnore]
        public bool usedParameter { get; set; }

        public string? ParameterFullTypeName { get; set; }

        [JsonIgnore]
        public Type? ParameterFullType { get; set; }

    }

    [Serializable]
    public enum enumDefaultUIValueType
    {
        Text,
        TextBox,
        DateTime,
        Date,
        Time,
        Number,
        Float,
        Combobox,
        List,
        Checkbox,
        Password,
        FileDialog,
        DirectoryDialog,
        Currency,
        Percent,
        Extension
    }

}
