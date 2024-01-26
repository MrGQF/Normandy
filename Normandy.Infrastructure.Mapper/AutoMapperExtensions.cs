using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Normandy.Infrastructure.Mapper
{
    public static class AutoMapperExtensions
    {
        //静态初始化
        private static IMapper _mapper = AutoMapperConfiguration.GetInstance().GetMapper();

        public static IEnumerable<TDes> MapTo<TSourse, TDes>(this IEnumerable<TSourse> Sourse)
            => Sourse.Select(sourse => sourse.MapTo<TSourse, TDes>());

        public static IQueryable<TDes> MapTo<TSourse, TDes>(this IQueryable<TSourse> Sourse)
            => Sourse.ProjectTo<TDes>(_mapper.ConfigurationProvider);

        public static TDes MapTo<TSourse, TDes>(this TSourse Sourse)
            => _mapper.Map<TSourse, TDes>(Sourse);

        public static TDes MapTo<TSourse, TDes>(this TSourse Sourse, TDes Des)
            => _mapper.Map(Sourse, Des);
    }
}
