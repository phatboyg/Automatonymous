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
namespace Automatonymous.StactIntegration.Tests
{
    using NUnit.Framework;
    using Stact;


    [TestFixture]
    public class A_simple_binding_to_a_state_machine
    {
        [Test]
        public void Should_initialize()
        {
            AutomatonymousActorFactory.Configure<MyInstance>(x =>
                {
                    x.ConstructedBy(inbox => new MyInstance(inbox));
                    x.UseStateMachine(() => new MyActor());
                });

            ActorRef actor = AutomatonymousActorFactory.New<MyInstance>();

            actor.Send(new CreateService());
        }


        class MyInstance :
            AutomatonymousActorInstance
        {
            readonly Inbox _inbox;

            public MyInstance(Inbox inbox)
            {
                _inbox = inbox;
            }

            public virtual State CurrentState { get; set; }

            public Inbox Inbox
            {
                get { return _inbox; }
            }
        }


        class MyActor :
            AutomatonymousActor<MyInstance>
        {
            public MyActor()
            {
                State(() => Running);

                Event(() => Create);

                During(Initial,
                    When(Create)
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }

            public Event<CreateService> Create { get; private set; }
        }


        class CreateService
        {
        }
    }
}