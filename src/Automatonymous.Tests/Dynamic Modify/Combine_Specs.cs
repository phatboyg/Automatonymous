﻿namespace Automatonymous.Tests.DynamicModify
{
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class When_combining_events_into_a_single_event
    {
        [Test]
        public async Task Should_have_called_combined_event()
        {
            _machine = CreateStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, Start);

            await _machine.RaiseEvent(_instance, First);
            await _machine.RaiseEvent(_instance, Second);

            Assert.IsTrue(_instance.Called);
        }

        [Test]
        public async Task Should_not_call_for_one_event()
        {
            _machine = CreateStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, Start);

            await _machine.RaiseEvent(_instance, First);

            Assert.IsFalse(_instance.Called);
        }

        [Test]
        public async Task Should_not_call_for_one_other_event()
        {
            _machine = CreateStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, Start);

            await _machine.RaiseEvent(_instance, Second);

            Assert.IsFalse(_instance.Called);
        }

        State Waiting;
        Event Start;
        Event First;
        Event Second;
        Event Third;

        StateMachine<Instance> _machine;
        Instance _instance;


        class Instance
        {
            public CompositeEventStatus CompositeStatus { get; set; }
            public bool Called { get; set; }
            public State CurrentState { get; set; }
        }

        private StateMachine<Instance> CreateStateMachine()
        {
            return AutomatonymousStateMachine<Instance>
                .New(builder => builder
                    .State("Waiting", out Waiting)
                    .Event("Start", out Start)
                    .Event("First", out First)
                    .Event("Second", out Second)
                    .Event("Third", out Third)
                    .CompositeEvent(Third, b => b.CompositeStatus, First, Second)
                    .Initially()
                        .When(Start, b => b.TransitionTo(Waiting))
                    .During(Waiting)
                        .When(Third, b => b
                            .Then(context => context.Instance.Called = true)
                            .Finalize()
                        )
                );
        }
    }


    [TestFixture(Category = "Dynamic Modify")]
    public class When_combining_events_with_an_int_for_state
    {
        [Test]
        public async Task Should_have_called_combined_event()
        {
            _machine = CreateStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, Start);

            Assert.IsFalse(_instance.Called);

            await _machine.RaiseEvent(_instance, First);
            await _machine.RaiseEvent(_instance, Second);

            Assert.IsTrue(_instance.Called);

            Assert.AreEqual(2, _instance.CurrentState);
        }

        [Test]
        public async Task Should_have_initial_state_with_zero()
        {
            _machine = CreateStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, Start);

            Assert.AreEqual(3, _instance.CurrentState);
        }

        [Test]
        public async Task Should_not_call_for_one_event()
        {
            _machine = CreateStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, Start);

            await _machine.RaiseEvent(_instance, First);

            Assert.IsFalse(_instance.Called);
        }

        [Test]
        public async Task Should_not_call_for_one_other_event()
        {
            _machine = CreateStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, Start);

            await _machine.RaiseEvent(_instance, Second);

            Assert.IsFalse(_instance.Called);
        }

        State Waiting;
        Event Start;
        Event First;
        Event Second;
        Event Third;

        StateMachine<Instance> _machine;
        Instance _instance;


        class Instance
        {
            public int CompositeStatus { get; set; }
            public bool Called { get; set; }
            public int CurrentState { get; set; }
        }

        private StateMachine<Instance> CreateStateMachine()
        {
            return AutomatonymousStateMachine<Instance>
                .New(builder => builder
                    .State("Waiting", out Waiting)
                    .Event("Start", out Start)
                    .Event("First", out First)
                    .Event("Second", out Second)
                    .Event("Third", out Third)
                    .InstanceState(b => b.CurrentState)
                    .CompositeEvent(Third, b => b.CompositeStatus, First, Second)
                    .Initially()
                        .When(Start, b => b.TransitionTo(Waiting))
                    .During(Waiting)
                        .When(Third, b => b
                            .Then(context => context.Instance.Called = true)
                            .Finalize()
                        )
                );
        }
    }
}
