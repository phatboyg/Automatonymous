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
    using Stact;


    public static class ActorInstanceConfiguratorExtensions
    {
        public static void UseStateMachine<TInstance, TMachine>(this ActorInstanceConfigurator<TInstance> configurator,
                                                                Func<TMachine> machineFactory)
            where TMachine : AutomatonymousActor<TInstance>
            where TInstance : class, AutomatonymousActorInstance
        {
            configurator.UseStateMachine<TMachine>(x => x.ConstructedBy(machineFactory));
        }

        public static void ConstructedBy<TInstance>(this ActorInstanceConfigurator<TInstance> configurator,
                                                    Func<TInstance> instanceFactory)
            where TInstance : class, AutomatonymousActorInstance
        {
            configurator.ConstructedBy((i, f, s) => instanceFactory());
        }

        public static void ConstructedBy<TInstance>(this ActorInstanceConfigurator<TInstance> configurator,
                                                    Func<Inbox, TInstance> instanceFactory)
            where TInstance : class, AutomatonymousActorInstance
        {
            configurator.ConstructedBy((i, f, s) => instanceFactory(i));
        }

        public static void ConstructedBy<TInstance>(this ActorInstanceConfigurator<TInstance> configurator,
                                                    Func<Inbox, Fiber, TInstance> instanceFactory)
            where TInstance : class, AutomatonymousActorInstance
        {
            configurator.ConstructedBy((i, f, s) => instanceFactory(i, f));
        }
    }
}