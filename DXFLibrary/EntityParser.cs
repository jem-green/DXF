﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DXFLibrary
{
    class EntityParser : ISectionParser
    {
        private Dictionary<string, Type> Entities = new Dictionary<string, Type>();
        private Entity currentEntity = null;
        private Stack<Entity> stack = new Stack<Entity>();
        #region ISectionParser Member

        public void ParseGroupCode(Document doc, int groupcode, string value)
        {
            if (Entities.Count == 0)
            {
                foreach (Type t in Assembly.GetCallingAssembly().GetTypes())
                {
                    if (t.IsClass && !t.IsAbstract)
                    {
                        object[] attrs = t.GetCustomAttributes(false);
                        foreach(object attr in attrs)
                        {
                            EntityAttribute casted = attr as EntityAttribute;
                            if (casted != null)
                            {
                                Entities.Add(casted.EntityName, t);
                            }
                        }
                    }
                }
            }
            if (groupcode == 0)
            {
                if (value == "SEQEND")
                {
                    if (stack.Count != 0)
                        currentEntity = stack.Pop();
                }
                if (Entities.ContainsKey(value))
                {
                    if (currentEntity!=null && currentEntity.HasChildren)
                    {
                        stack.Push(currentEntity);
                    }
                    currentEntity = Activator.CreateInstance(Entities[value]) as Entity;
                    if (stack.Count>0 && stack.Peek().HasChildren)
                    {
                        stack.Peek().Children.Add(currentEntity);
                    }
                    else
                    {
                        doc.Entities.Add(currentEntity);
                    }
                }
                else
                {
                    currentEntity = null;
                    //TODO: warning
                }
            }
            if (currentEntity != null)
            {
                currentEntity.ParseGroupCode(groupcode, value);
            }
        }

        #endregion
    }
}
