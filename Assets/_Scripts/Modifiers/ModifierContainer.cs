using System.Collections.Generic;

namespace Project.Modifiers
{
    public class ModifierContainer<T, U> where T : Modifier<U>
    {
        private readonly List<T> Modifiers = new List<T>();

        public void AddModifier(T modifier) => Modifiers.Add(modifier);

        public void RemoveModifier(T modifier) => Modifiers.Remove(modifier);

        public U ApplyModifiers(U initialValue)
        {
            U modifiedValue = initialValue;

            foreach (T modifier in Modifiers)
            {
                modifiedValue = modifier.ModifyValue(modifiedValue);
            }

            return modifiedValue;
        }
    }

    public abstract class Modifier
    {

    }

    public abstract class Modifier<T> : Modifier
    {
        public abstract T ModifyValue(T value);
    }
}