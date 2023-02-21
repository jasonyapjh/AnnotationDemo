using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnotationOpenSource.Events
{
    public class PrerequisiteModulesLoaded : PubSubEvent { }
    public class AllModulesLoaded : PubSubEvent { }
    public class ApplicationCloseEvent : PubSubEvent { }
}
