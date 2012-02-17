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
    using Magnum.Caching;
    using Magnum.Extensions;
    using Stact;


    public static class AutomatonymousActorFactory
    {
        static GenericTypeCache<AutomatonymousActorInstanceFactory> _factories;

        static AutomatonymousActorFactory()
        {
            _factories =
                new GenericTypeCache<AutomatonymousActorInstanceFactory>(typeof(AutomatonymousActorInstanceFactory<>));
        }

        public static void Configure<TInstance>(Action<ActorInstanceConfigurator<TInstance>> configureCallback)
            where TInstance : class, AutomatonymousActorInstance
        {
            if (_factories.Has(typeof(TInstance)))
            {
                throw new StactException("The actor factory for instance type " + typeof(TInstance).ToShortTypeName()
                                         + " was already configured");
            }

            var configurator = new ActorInstanceConfiguratorImpl<TInstance>();

            configureCallback(configurator);

            AutomatonymousActorInstanceFactory factory = configurator.Configure();

            _factories.Add(typeof(TInstance), factory);
        }

        public static AutomatonymousActorInstanceFactory<TInstance> GetFactory<TInstance>()
            where TInstance : ActorStateMachine
        {
            AutomatonymousActorInstanceFactory factory = _factories[typeof(TInstance)];

            return factory as AutomatonymousActorInstanceFactory<TInstance>;
        }

        public static ActorRef New<TInstance>()
            where TInstance : AutomatonymousActorInstance
        {
            AutomatonymousActorInstanceFactory factory = _factories[typeof(TInstance)];

            return factory.New();
        }
    }
}