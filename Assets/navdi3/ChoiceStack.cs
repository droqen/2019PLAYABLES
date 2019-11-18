namespace navdi3
{

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class ChoiceStack<T> : IEnumerable<T>
    {
        bool locked = true;
        int lockIndex = 0;

        public List<T> choices;

        public ChoiceStack()
        {
            choices = new List<T>();
        }
        public ChoiceStack(ICollection<T> items)
        {
            choices = new List<T>();
            AddManyThenLock(items);
        }
        public ChoiceStack(params T[] items)
        {
            choices = new List<T>();
            AddManyThenLock(items);
        }

        public void AddManyThenLock(ICollection<T> items)
        {
            foreach (var item in items) Add(item);
            Lock();
        }
        public void AddManyThenLock(params T[] items)
        {
            for (var i = 0; i < items.Length; i++) Add(items[i]);
            Lock();
        }

        public void Add(T item)
        {
            choices.Add(item);
        }

        public void Lock(bool shuffle = true)
        {
            if (shuffle) Util.shufl(ref choices, start: lockIndex);
            locked = true;
            lockIndex = choices.Count;
        }

        public T GetRandom()
        {
            return choices[Random.Range(0, choices.Count)];
        }

        public T GetFirstTrue(System.Func<T,bool> func, T defaultValue = default(T))
        {
            if (!locked) throw new System.Exception("ChoiceStack is not locked. Can't GetFirstTrue; call Lock() first");
            foreach (var choice in choices) if (func(choice)) return choice;
            return defaultValue;
        }
        public T GetHighest(System.Func<T, float> func, T defaultValue = default(T))
        {
            T highestItem = default(T);
            float highestItemsValue = float.MinValue;

            foreach (var choice in choices) {
                var choiceValue = func(choice);
                if (choiceValue > highestItemsValue)
                {
                    highestItem = choice;
                    highestItemsValue = choiceValue;
                }
            }

            if (highestItemsValue == float.MinValue) throw new System.Exception("ChoiceStack is empty OR all items are valued at MinValue (don't do this); GetHighest failed");

            return highestItem;
        }
        // Same as above but for integers.
        public T GetHighest(System.Func<T, int> func, T defaultValue = default(T))
        {
            T highestItem = default(T);
            int highestItemsValue = int.MinValue;

            foreach (var choice in choices)
            {
                var choiceValue = func(choice);
                if (choiceValue > highestItemsValue)
                {
                    highestItem = choice;
                    highestItemsValue = choiceValue;
                }
            }

            if (highestItemsValue == int.MinValue) throw new System.Exception("ChoiceStack is empty OR all items are valued at MinValue (don't do this); GetHighest failed");

            return highestItem;
        }

        public void RemoveAll(T item)
        {
            if (!locked) throw new System.Exception("ChoiceStack is not locked. Can't RemoveAll; call Lock() first");
            choices.RemoveAll((T x) => { return item.Equals(x); });
            lockIndex = choices.Count;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (!locked) throw new System.Exception("ChoiceStack is not locked. Can't enumerate; call Lock() first");
            return ((IEnumerable<T>)choices).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!locked) throw new System.Exception("ChoiceStack is not locked. Can't enumerate; call Lock() first");
            return ((IEnumerable<T>)choices).GetEnumerator();
        }
    }

}