using FluentNHibernate;
using FluentNHibernate.Mapping;
using Logic.Entities;

namespace Logic.Mappings
{
    public class MovieMap : ClassMap<Movie>
    {
        public MovieMap()
        {
            Id(x => x.Id);

            Map(x => x.Name);
            //Map(x => x.LicensingModel).CustomType<int>();
            Map(Reveal.Member<Movie>("LicensingModel")).CustomType<int>(); // because of protected prefix

            DiscriminateSubClassesOnColumn("LicensingModel");
        }
    }

    public class TwoDaysMovieMap : SubclassMap<Movie>
    {
        public TwoDaysMovieMap()
        {
            DiscriminatorValue(1); 
        }

        /*
         
            public enum LicensingModel
            {
                TwoDays = 1,
                LifeLong = 2
            }
         */
    }

    public class LifeLongMovieMap : SubclassMap<Movie>
    {
        public LifeLongMovieMap()
        {
            DiscriminatorValue(2);
        }

        /*
       
          public enum LicensingModel
          {
              TwoDays = 1,
              LifeLong = 2
          }
         */
    }
}
