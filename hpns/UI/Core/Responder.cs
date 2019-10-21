using System;
using System.Collections.Generic;
using System.Dynamic;

namespace HPNS.UI.Core
{
    public class Responder
    {
        private class CommandHandler
        {
            public string CommandName { get; }

            public Action<dynamic> Handler { get; }

            public CommandHandler(string commandName, Action<dynamic> handler)
            {
                CommandName = commandName;
                Handler = handler;
            }
        }
        
        private List<CommandHandler> _handlers = new List<CommandHandler>();
        private List<Responder> _children = new List<Responder>();

        public string Name { get; }
        public Responder Parent { get; set; }

        public Responder(string name)
        {
            Name = name;
            
            RegisterHandler("propagate", data => Propagate(data.target, data.command));
        }

        public void Handle(dynamic command)
        {
            foreach (var handler in _handlers)
            {
                if (handler.CommandName != command.name) continue;

                handler.Handler(DoesPropertyExist(command, "data") ? command.data : null);
                return;
            }
            
            throw new Exception($"Handler is not found for command '{command.name}'.");
        }

        public void AddChild(Responder child)
        {
            _children.Add(child);
            child.Parent = this;
        }

        protected virtual bool Reply(object command)
        {
            if (Parent == null)
                throw new Exception("Cannot send reply because there is no appropriate parent.");

            return Parent.Reply(new
            {
                name = "propagate",
                data = new {target = Name, command = command}
            });
        }

        protected void RegisterHandler(string commandName, Action<dynamic> handler)
        {
            _handlers.Add(new CommandHandler(commandName, handler));
        }

        private void Propagate(string target, dynamic command)
        {
            foreach (var child in _children)
            {
                if (child.Name != target) continue;

                child.Handle(command);
                return;
            }

            throw new Exception($"Target {target} is not found for propagation.");
        }
        
        private static bool DoesPropertyExist(dynamic obj, string name)
        {
            if (obj is ExpandoObject)
                return ((IDictionary<string, object>) obj).ContainsKey(name);

            return obj.GetType().GetProperty(name) != null;
        }
    }
}