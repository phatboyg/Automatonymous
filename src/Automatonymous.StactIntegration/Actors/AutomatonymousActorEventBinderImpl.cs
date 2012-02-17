// Copyright 2011 Chris Patterson, Dru Sellers
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Automatonymous.StactIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Stact;

    public class AutomatonymousActorEventBinderImpl<TInstance, TData> :
        AutomatonymousActorEventBinder<TInstance>
        where TInstance : class, AutomatonymousActorInstance
        where TData : class
    {
        readonly Event<TData> _event;
        readonly EventRaiser<TInstance, TData> _raiser;
        readonly HashSet<State> _states;
        StateMachine<TInstance> _machine;

        public AutomatonymousActorEventBinderImpl(StateMachine<TInstance> machine, Event<TData> @event)
        {
            _machine = machine;
            _event = @event;
            _states = new HashSet<State>(machine.States.Where(state => machine.NextEvents(state).Contains(@event)));
            _raiser = _machine.CreateEventRaiser(_event);
        }

        public Type DataType
        {
            get { return typeof(TData); }
        }

        public Event Event
        {
            get { return _event; }
        }

        public IEnumerable<State> States
        {
            get { return _states; }
        }

        public ReceiveLoop Bind(ReceiveLoop loop, TInstance instance)
        {
            SelectiveConsumer<TData> consumer = message =>
                {
                    State currentState = instance.CurrentState;

                    if (!_states.Contains(currentState))
                        return null;

                    return m =>
                        {
                            try
                            {
                                _raiser.Raise(instance, m);
                            }
                            finally
                            {
                                loop.Continue();
                            }
                        };
                };

            return loop.Receive(consumer);
        }
    }
}