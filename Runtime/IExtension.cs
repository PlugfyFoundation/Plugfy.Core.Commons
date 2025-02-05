using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Plugfy.Core.Commons.Runtime
{
    public interface IExtension
    {
        IReadOnlyCollection<ExecutionOption> ExecutionOptions { get; }

        void Execute(ExecutionOption? executionOption, dynamic executionParameters, EventHandler eventData);
    }

    public class ExecutionOption
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public Type Type { get; init; }
        public bool IsRequired { get; init; }
        public string HelpText { get; init; }
        public ICollection<ExecutionParameter>? Parameters { get; init; }
    }


    public class ExecutionParameter
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public Type Type { get; init; }
        public bool IsRequired { get; init; }
        public string HelpText { get; init; }
    }

}
