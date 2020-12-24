namespace Automatonymous.Binders
{
    using System;
    using System.Threading.Tasks;
    using Activities;
    using Behaviors;


    public class ConditionalExceptionActivityBinder<TInstance, TException> :
        ActivityBinder<TInstance>
        where TInstance : class
        where TException : Exception
    {
        readonly StateMachineAsyncExceptionCondition<TInstance, TException> _condition;
        readonly EventActivities<TInstance> _elseActivities;
        readonly Event _event;
        readonly EventActivities<TInstance> _thenActivities;

        public ConditionalExceptionActivityBinder(Event @event, StateMachineExceptionCondition<TInstance, TException> condition,
            EventActivities<TInstance> thenActivities, EventActivities<TInstance> elseActivities)
            : this(@event, context => Task.FromResult(condition(context)), thenActivities, elseActivities)
        {
        }

        public ConditionalExceptionActivityBinder(Event @event, StateMachineAsyncExceptionCondition<TInstance, TException> condition,
            EventActivities<TInstance> thenActivities, EventActivities<TInstance> elseActivities)
        {
            _thenActivities = thenActivities;
            _elseActivities = elseActivities;
            _condition = condition;
            _event = @event;
        }

        public bool IsStateTransitionEvent(State state)
        {
            return Equals(_event, state.Enter) || Equals(_event, state.BeforeEnter)
                   || Equals(_event, state.AfterLeave) || Equals(_event, state.Leave);
        }

        public void Bind(State<TInstance> state)
        {
            var thenBehavior = GetBehavior(_thenActivities);
            var elseBehavior = GetBehavior(_elseActivities);

            var conditionActivity = new ConditionExceptionActivity<TInstance, TException>(_condition, thenBehavior, elseBehavior);

            state.Bind(_event, conditionActivity);
        }

        public void Bind(BehaviorBuilder<TInstance> builder)
        {
            var thenBehavior = GetBehavior(_thenActivities);
            var elseBehavior = GetBehavior(_elseActivities);

            var conditionActivity = new ConditionExceptionActivity<TInstance, TException>(_condition, thenBehavior, elseBehavior);

            builder.Add(conditionActivity);
        }

        Behavior<TInstance> GetBehavior(EventActivities<TInstance> activities)
        {
            var catchBuilder = new CatchBehaviorBuilder<TInstance>();

            foreach (var activity in activities.GetStateActivityBinders())
                activity.Bind(catchBuilder);
            return catchBuilder.Behavior;
        }
    }


    public class ConditionalExceptionActivityBinder<TInstance, TData, TException> :
        ActivityBinder<TInstance>
        where TInstance : class
        where TException : Exception
    {
        readonly StateMachineAsyncExceptionCondition<TInstance, TData, TException> _condition;
        readonly EventActivities<TInstance> _elseActivities;
        readonly Event _event;
        readonly EventActivities<TInstance> _thenActivities;

        public ConditionalExceptionActivityBinder(Event @event, StateMachineExceptionCondition<TInstance, TData, TException> condition,
            EventActivities<TInstance> thenActivities, EventActivities<TInstance> elseActivities)
            : this(@event, context => Task.FromResult(condition(context)), thenActivities, elseActivities)
        {
        }

        public ConditionalExceptionActivityBinder(Event @event, StateMachineAsyncExceptionCondition<TInstance, TData, TException> condition,
            EventActivities<TInstance> thenActivities, EventActivities<TInstance> elseActivities)
        {
            _thenActivities = thenActivities;
            _elseActivities = elseActivities;
            _condition = condition;
            _event = @event;
        }

        public bool IsStateTransitionEvent(State state)
        {
            return Equals(_event, state.Enter) || Equals(_event, state.BeforeEnter)
                   || Equals(_event, state.AfterLeave) || Equals(_event, state.Leave);
        }

        public void Bind(State<TInstance> state)
        {
            var thenBehavior = GetBehavior(_thenActivities);
            var elseBehavior = GetBehavior(_elseActivities);

            var conditionActivity = new ConditionExceptionActivity<TInstance, TData, TException>(_condition, thenBehavior, elseBehavior);

            state.Bind(_event, conditionActivity);
        }

        public void Bind(BehaviorBuilder<TInstance> builder)
        {
            var thenBehavior = GetBehavior(_thenActivities);
            var elseBehavior = GetBehavior(_elseActivities);

            var conditionActivity = new ConditionExceptionActivity<TInstance, TData, TException>(_condition, thenBehavior, elseBehavior);

            builder.Add(conditionActivity);
        }

        Behavior<TInstance> GetBehavior(EventActivities<TInstance> activities)
        {
            var catchBuilder = new CatchBehaviorBuilder<TInstance>();

            foreach (var activity in activities.GetStateActivityBinders())
                activity.Bind(catchBuilder);

            return catchBuilder.Behavior;
        }
    }
}
