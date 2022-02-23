using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// Required because MonoBehaviours cannot have template semantics... kekw
public interface IStateMgr
{
    void OnTick(Time t);
    void OnStop(Time t);
}

public partial class StateManager<T> : IStateMgr where T : Enum
{
    abstract class SMTransition
    {
        public SMTransition(T toState, StateManager<T> stateMgr)
        {
            this.targetState = toState;
            this.stateMgr = stateMgr;
        }
        protected T targetState;
        protected StateManager<T> stateMgr;
        public abstract bool CheckTransition(Time t);
    }
    public StateManager()
    {
        _starts = new Dictionary<T, HashSet<Action<Time>>>();
        _ticks = new Dictionary<T, HashSet<Action<Time>>>();
        _cleanups = new Dictionary<T, HashSet<Action<Time>>>();
        _transitions = new List<SMTransition>();
        _allStates = new HashSet<T>();
        _rngStates = new List<T>();
        T[] states = (T[])Enum.GetValues(typeof(T));
        foreach (T state in states)
        {
            _starts[state] = new HashSet<Action<Time>>();
            _ticks[state] = new HashSet<Action<Time>>();
            _cleanups[state] = new HashSet<Action<Time>>();
        }
    }

    public T State => _state;
    private T _state;
    private bool _tickedState;
    private Dictionary<T, HashSet<Action<Time>>> _starts;
    private Dictionary<T, HashSet<Action<Time>>> _ticks;
    private Dictionary<T, HashSet<Action<Time>>> _cleanups;

    private List<SMTransition> _transitions;

    private HashSet<T> _allStates;
    private List<T> _rngStates;

    public StateManager<T> Register(T state, Action<Time> tick = null, Action<Time> start = null, Action<Time> cleanup = null)
    {
        if (start != null)
            _starts[state].Add(start);
        if (tick != null)
            _ticks[state].Add(tick);
        if (cleanup != null)
            _cleanups[state].Add(cleanup);
        return this;
    }
    public StateManager<T> Unregister(T state, Action<Time> tick = null, Action<Time> start = null, Action<Time> cleanup = null)
    {
        if (start != null)
            _starts[state].Remove(start);
        if (tick != null)
            _ticks[state].Remove(tick);
        if (cleanup != null)
            _cleanups[state].Remove(cleanup);
        return this;
    }
    public StateManager<T> Clear(T state)
    {
        _starts[state].Clear();
        _ticks[state].Clear();
        _cleanups[state].Clear();
        return this;
    }
    public StateManager<T> Transition(T state, Time t)
    {
        if (_tickedState)
        {
            HashSet<Action<Time>> cleanups = _cleanups[_state];
            foreach (var cleanup in cleanups.ToArray())
                cleanup(t);
            _tickedState = false;
        }
        _state = state;
        return this;
    }

    // Unity-specific object update hooking
    public void Attach(MonoBehaviour parent)
    {
        var stateRunner = parent.gameObject.AddComponent<StateRunner>();
        stateRunner.StateMgr = this;
    }

    // implementation of 7-bag (or n-bag) from tetris.
    public StateManager<T> BagState(params T[] states)
    {
        foreach (T state in states)
            _allStates.Add(state);
        return this;
    }
    public StateManager<T> RemoveFromBag(params T[] states)
    {
        foreach (T state in states)
        {
            _allStates.Remove(state);
            _rngStates.Remove(state);
        }
        return this;
    }
    public StateManager<T> ClearBag()
    {
        // clean slate
        _allStates.Clear();
        _rngStates.Clear();
        return this;
    }
    //public void RandomTransitionFromBag(Time t, bool allowDoubleUp = false)
    //{
    //    if (_allStates.Count == 0)
    //    {
    //        Debug.LogError("DISASTER!! Tried to call RandomTransitionFromBag before calling BagState()...");
    //        return;
    //    }

    //    // reset and re-fill the bag with
    //    // the bagged states in a randomized order.
    //    if (_rngStates.Count == 0)
    //    {
    //        _rngStates = _allStates.ToList();
    //        Host.Owner.Shuffle(_rngStates);
    //    }

    //    // If we don't allow doubling-up,
    //    // get the index of the current state
    //    int curStateIndex = allowDoubleUp ? -1 :
    //        _rngStates.FindIndex(_ => _.Equals(_state));
    //    // Get a random index, excluding the current state
    //    // if we don't allow doubling up
    //    int random = curStateIndex == -1 ?
    //        Host.Owner.GetRandomInt(_rngStates.Count) :
    //        Host.Owner.GetRandomExcept(_rngStates.Count, curStateIndex);

    //    // Pop the new state out and transition.
    //    T state = _rngStates[random];
    //    _rngStates.RemoveAt(random);
    //    Transition(state, t);
    //}

    public void OnTick(Time t)
    {
        // check auto-transitions
        foreach (var transition in _transitions.ToArray())
        {
            if (transition.CheckTransition(t))
                _transitions.Remove(transition);
        }
        // hold this state value in-case we
        // transition in a `start` listener
        T state = _state;
        if (!_tickedState)
        {
            HashSet<Action<Time>> starts = _starts[state];
            foreach (var start in starts.ToArray())
                start(t);
            _tickedState = true;
        }
        HashSet<Action<Time>> ticks = _ticks[state];
        foreach (var tick in ticks.ToArray())
            tick(t);
    }

    public void OnStop(Time t)
    {
        if (_tickedState)
        {
            HashSet<Action<Time>> cleanups = _cleanups[_state];
            foreach (var cleanup in cleanups.ToArray())
                cleanup(t);
            _tickedState = false;
        }
    }
}