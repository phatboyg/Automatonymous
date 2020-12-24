﻿namespace Automatonymous.Tests.DynamicModify
{
    namespace Telephone_Sample
    {
        using System;
        using System.Diagnostics;
        using System.Threading.Tasks;
        using Graphing;
        using GreenPipes;
        using GreenPipes.Introspection;
        using NUnit.Framework;
        using Visualizer;


        [TestFixture(Category = "Dynamic Modify")]
        public class A_simple_phone_call
        {
            [Test]
            public async Task Should_be_short_and_sweet()
            {
                var phone = new PrincessModelTelephone();
                await _machine.RaiseEvent(phone, _model.ServiceEstablished, new PhoneServiceEstablished {Digits = "555-1212"});

                await _machine.RaiseEvent(phone, _model.CallDialed);
                await _machine.RaiseEvent(phone, _model.CallConnected);

                await Task.Delay(50);

                await _machine.RaiseEvent(phone, _model.HungUp);

                Assert.AreEqual(_model.OffHook.Name, phone.CurrentState);
                Assert.GreaterOrEqual(phone.CallTimer.ElapsedMilliseconds, 45);
            }

            PhoneServiceStateModel _model;
            StateMachine<PrincessModelTelephone> _machine;

            [OneTimeSetUp]
            public void Setup()
            {
                _model = new PhoneServiceStateModel();
                _machine = _model.Machine;
            }
        }


        [TestFixture(Category = "Dynamic Modify")]
        public class Visualize
        {
            [Test]
            public void Draw()
            {
                var machine = new PhoneServiceStateModel().Machine;
                var generator = new StateMachineGraphvizGenerator(machine.GetGraph());

                var dotFile = generator.CreateDotFile();

                Console.WriteLine(dotFile);
            }

            [Test]
            public void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
            {
                ProbeResult result = new PhoneServiceStateModel().Machine.GetProbeResult();

                Console.WriteLine(result.ToJsonString());
            }

                        PhoneServiceStateModel _model;
            StateMachine<PrincessModelTelephone> _machine;

            [OneTimeSetUp]
            public void Setup()
            {
                _model = new PhoneServiceStateModel();
                _machine = _model.Machine;
            }
        }


        [TestFixture(Category = "Dynamic Modify")]
        public class A_short_time_on_hold
        {
            [Test]
            public async Task Should_be_short_and_sweet()
            {
                var phone = new PrincessModelTelephone();
                await _machine.RaiseEvent(phone, _model.ServiceEstablished, new PhoneServiceEstablished {Digits = "555-1212"});

                await _machine.RaiseEvent(phone, _model.CallDialed);
                await _machine.RaiseEvent(phone, _model.CallConnected);

                await Task.Delay(50);

                await _machine.RaiseEvent(phone, _model.PlacedOnHold);
                await _machine.RaiseEvent(phone, _model.TakenOffHold);
                await _machine.RaiseEvent(phone, _model.HungUp);

                Assert.AreEqual(_model.OffHook.Name, phone.CurrentState);
                Assert.GreaterOrEqual(phone.CallTimer.ElapsedMilliseconds, 45);
            }

            PhoneServiceStateModel _model;
            StateMachine<PrincessModelTelephone> _machine;

            [OneTimeSetUp]
            public void Setup()
            {
                _model = new PhoneServiceStateModel();
                _machine = _model.Machine;
            }
        }


        [TestFixture(Category = "Dynamic Modify")]
        public class An_extended_time_on_hold
        {
            [Test]
            public async Task Should_end__badly()
            {
                var phone = new PrincessModelTelephone();
                await _machine.RaiseEvent(phone, _model.ServiceEstablished, new PhoneServiceEstablished {Digits = "555-1212"});

                await _machine.RaiseEvent(phone, _model.CallDialed);
                await _machine.RaiseEvent(phone, _model.CallConnected);
                await _machine.RaiseEvent(phone, _model.PlacedOnHold);

                await Task.Delay(50);

                await _machine.RaiseEvent(phone, _model.HungUp);

                Assert.AreEqual(_model.OffHook.Name, phone.CurrentState);
                Assert.GreaterOrEqual(phone.CallTimer.ElapsedMilliseconds, 45);
            }

            PhoneServiceStateModel _model;
            StateMachine<PrincessModelTelephone> _machine;

            [OneTimeSetUp]
            public void Setup()
            {
                _model = new PhoneServiceStateModel();
                _machine = _model.Machine;
            }
        }


        class PrincessModelTelephone
        {
            public PrincessModelTelephone()
            {
                CallTimer = new Stopwatch();
            }

            public string CurrentState { get; set; }

            public Stopwatch CallTimer { get; private set; }

            public string Number { get; set; }
        }

        class PhoneServiceEstablished
        {
            public string Digits { get; set; }
        }

        class PhoneServiceStateModel
        {
            public StateMachine<PrincessModelTelephone> Machine;

            public PhoneServiceStateModel()
            {
                Machine = CreateDynamically();
            }

            public State OffHook;
            public State Ringing;
            public State<PrincessModelTelephone> Connected;
            public State<PrincessModelTelephone> OnHold;
            public State PhoneDestroyed;

            public Event<PhoneServiceEstablished> ServiceEstablished;
            public Event CallDialed;
            public Event HungUp;
            public Event CallConnected;
            public Event LeftMessage;
            public Event PlacedOnHold;
            public Event TakenOffHold;
            public Event PhoneHurledAgainstWall;

            void StopCallTimer(PrincessModelTelephone instance)
            {
                instance.CallTimer.Stop();

                Console.WriteLine("Stopped call timer at {0}ms", instance.CallTimer.ElapsedMilliseconds);
            }

            void StartCallTimer(PrincessModelTelephone instance)
            {
                Console.WriteLine("Started call timer");

                instance.CallTimer.Start();
            }

            public StateMachine<PrincessModelTelephone> CreateDynamically()
            {
                return AutomatonymousStateMachine<PrincessModelTelephone>
                    .New(builder => builder
                        .State("OffHook", out OffHook)
                        .State("Ringing", out Ringing)
                        .State("Connected", out Connected)
                        .State("PhoneDestroyed", out PhoneDestroyed)
                        .Event("ServiceEstablished", out ServiceEstablished)
                        .Event("CallDialed", out CallDialed)
                        .Event("HungUp", out HungUp)
                        .Event("CallConnected", out CallConnected)
                        .Event("LeftMessage", out LeftMessage)
                        .Event("PlacedOnHold", out PlacedOnHold)
                        .Event("TakenOffHold", out TakenOffHold)
                        .Event("PhoneHurledAgainstWall", out PhoneHurledAgainstWall)
                        .InstanceState(x => x.CurrentState)
                        .SubState("OnHold", Connected, out OnHold)
                        .Initially()
                            .When(ServiceEstablished, b => b
                                .Then(context => context.Instance.Number = context.Data.Digits)
                                .TransitionTo(OffHook))
                        .During(OffHook)
                            .When(CallDialed, b => b.TransitionTo(Ringing))
                        .During(Ringing)
                            .When(HungUp, b => b.TransitionTo(OffHook))
                            .When(CallConnected, b => b.TransitionTo(Connected))
                        .During(Connected)
                            .When(LeftMessage, b => b.TransitionTo(OffHook))
                            .When(HungUp, b => b.TransitionTo(OffHook))
                            .When(PlacedOnHold, b => b.TransitionTo(OnHold))
                        .During(OnHold)
                            .When(TakenOffHold, b => b.TransitionTo(Connected))
                            .When(PhoneHurledAgainstWall, b => b.TransitionTo(PhoneDestroyed))
                        .DuringAny()
                            .When(Connected.Enter, b => b.Then(context => StartCallTimer(context.Instance)))
                            .When(Connected.Leave, b => b.Then(context => StopCallTimer(context.Instance)))
                    );
            }
        }
    }
}
