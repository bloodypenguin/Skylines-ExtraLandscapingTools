using System;

namespace NaturalResourcesBrush.Redirection
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RedirectReverseAttribute : Attribute
    {
        public RedirectReverseAttribute()
        {
            this.OnCreated = false;
        }

        public RedirectReverseAttribute(bool onCreated)
        {
            this.OnCreated = onCreated;
        }

        public bool OnCreated { get; }
    }
}