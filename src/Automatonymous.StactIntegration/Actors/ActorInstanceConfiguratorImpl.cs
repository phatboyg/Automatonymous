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
    using Magnum.Caching;
    using Magnum.Reflection;
    using Stact;
    using Stact.Configuration;


    public class ActorInstanceConfiguratorImpl<TInstance> :
        ActorInstanceConfigurator<TInstance>,
        Configurator
        where TInstance : class, AutomatonymousActorInstance
    {
        Configurator _machineConfigurator;
        Func<StateMachine<TInstance>> _machineFactory;
        Func<Inbox, Fiber, Scheduler, TInstance> _instanceFactory;

        public void ValidateConfiguration()
        {
            if(_machineConfigurator == null)
                throw new StactException(
                    "No machine factory was configured, call UseStateMachine to configure the state machine type");

            var machine = _machineFactory();

            var binders = CreateStateMachineReceiveBehaviors(machine);
        }

        IEnumerable<AutomatonymousActorEventBinder<TInstance>> CreateStateMachineReceiveBehaviors(StateMachine<TInstance> machine)
        {

            foreach (Event @event in machine.Events)
            {
                Type eventType = @event.GetType();

                Type dataEventInterfaceType = eventType.GetInterfaces()
                    .Where(x => x.IsGenericType)
                    .Where(x => x.GetGenericTypeDefinition() == typeof(Event<>))
                    .SingleOrDefault();
                if (dataEventInterfaceType == null)
                    continue;

                Type dataType = dataEventInterfaceType.GetGenericArguments()[0];

                var binder = type => (AutomatonymousActorEventBinderFactory<TInstance>)
                FastActivator.Create(typeof(AutomatonymousActorEventBinderFactoryImpl<,>),
                    new[] { typeof(TInstance), type }, new object[]{machine}));



                yield return factory.Create(states);
            }

        }

        public AutomatonymousActorInstanceFactory Configure()
        {
            var machine = _machineFactory
        }

        public void UseStateMachine<TActor>(Action<AutomatonymousActorConfigurator<TActor, TInstance>> configureCallback)
            where TActor : ActorStateMachine<TInstance>
        {
            var configurator = new AutomatonymousActorConfiguratorImpl<TActor, TInstance>();
            configureCallback(configurator);

            _machineConfigurator = configurator;
            _machineFactory = configurator.Configure;
        }

        public void ConstructedBy(Func<Inbox,Fiber,Scheduler,TInstance> instanceFactory)
        {
            _instanceFactory = (i,f,s) => instanceFactory(i);
        }
    }
}