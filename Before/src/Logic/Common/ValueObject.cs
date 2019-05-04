namespace Logic.Entities
{
    public abstract class ValueObject<T> where T: ValueObject<T>
    {
        protected abstract bool EqualsCore(T other);
      

        public override bool Equals(object obj)
        {
            var vo = obj as T;

            if (ReferenceEquals(null, vo)) return false;
            //if (ReferenceEquals(this, vo)) return true;
            if (obj.GetType() != this.GetType()) return false;

            return EqualsCore(vo);
        }

        public override int GetHashCode()
        {
            return GetHashCodeCore();
        }

        protected abstract int GetHashCodeCore();

        public static bool operator ==(ValueObject<T> left, ValueObject<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ValueObject<T> left, ValueObject<T> right)
        {
            return !Equals(left, right);
        }
    }
}