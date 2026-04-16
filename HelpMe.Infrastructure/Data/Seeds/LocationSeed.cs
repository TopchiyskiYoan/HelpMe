using HelpMe.Application.Interfaces;
using HelpMe.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Infrastructure.Data.Seeds;

public static class LocationSeed
{
    public static async Task SeedAsync(IApplicationDbContext context)
    {
        if (await context.Regions.AnyAsync())
            return;

        var regions = new List<Region>
        {
            new()
            {
                Name = "Благоевград",
                Cities =
                [
                    new() { Name = "Благоевград" }, new() { Name = "Банско" }, new() { Name = "Белица" },
                    new() { Name = "Гоце Делчев" }, new() { Name = "Добринище" }, new() { Name = "Кресна" },
                    new() { Name = "Мелник" }, new() { Name = "Петрич" }, new() { Name = "Разлог" },
                    new() { Name = "Сандански" }, new() { Name = "Симитли" }, new() { Name = "Хаджидимово" },
                    new() { Name = "Якоруда" }
                ]
            },
            new()
            {
                Name = "Бургас",
                Cities =
                [
                    new() { Name = "Бургас" }, new() { Name = "Айтос" }, new() { Name = "Ахелой" },
                    new() { Name = "Ахтопол" }, new() { Name = "Българово" }, new() { Name = "Каблешково" },
                    new() { Name = "Камено" }, new() { Name = "Карнобат" }, new() { Name = "Китен" },
                    new() { Name = "Несебър" }, new() { Name = "Обзор" }, new() { Name = "Поморие" },
                    new() { Name = "Приморско" }, new() { Name = "Свети Влас" }, new() { Name = "Созопол" },
                    new() { Name = "Средец" }, new() { Name = "Сунгурларе" }, new() { Name = "Царево" },
                    new() { Name = "Черноморец" }
                ]
            },
            new()
            {
                Name = "Варна",
                Cities =
                [
                    new() { Name = "Варна" }, new() { Name = "Аксаково" }, new() { Name = "Белослав" },
                    new() { Name = "Бяла" }, new() { Name = "Ветрино" }, new() { Name = "Вълчи дол" },
                    new() { Name = "Девня" }, new() { Name = "Долни чифлик" }, new() { Name = "Дългопол" },
                    new() { Name = "Игнатиево" }, new() { Name = "Провадия" }, new() { Name = "Суворово" }
                ]
            },
            new()
            {
                Name = "Велико Търново",
                Cities =
                [
                    new() { Name = "Велико Търново" }, new() { Name = "Бяла черква" }, new() { Name = "Горна Оряховица" },
                    new() { Name = "Дебелец" }, new() { Name = "Долна Оряховица" }, new() { Name = "Елена" },
                    new() { Name = "Златарица" }, new() { Name = "Килифарево" }, new() { Name = "Лясковец" },
                    new() { Name = "Павликени" }, new() { Name = "Полски Тръмбеш" }, new() { Name = "Стражица" },
                    new() { Name = "Сухиндол" }, new() { Name = "Свищов" }
                ]
            },
            new()
            {
                Name = "Видин",
                Cities =
                [
                    new() { Name = "Видин" }, new() { Name = "Белоградчик" }, new() { Name = "Брегово" },
                    new() { Name = "Грамада" }, new() { Name = "Димово" }, new() { Name = "Дунавци" },
                    new() { Name = "Кула" }
                ]
            },
            new()
            {
                Name = "Враца",
                Cities =
                [
                    new() { Name = "Враца" }, new() { Name = "Бяла Слатина" }, new() { Name = "Козлодуй" },
                    new() { Name = "Криводол" }, new() { Name = "Мездра" }, new() { Name = "Мизия" },
                    new() { Name = "Оряхово" }, new() { Name = "Роман" }
                ]
            },
            new()
            {
                Name = "Габрово",
                Cities =
                [
                    new() { Name = "Габрово" }, new() { Name = "Дряново" }, new() { Name = "Плачковци" },
                    new() { Name = "Севлиево" }, new() { Name = "Трявна" }
                ]
            },
            new()
            {
                Name = "Добрич",
                Cities =
                [
                    new() { Name = "Добрич" }, new() { Name = "Балчик" }, new() { Name = "Генерал Тошево" },
                    new() { Name = "Каварна" }, new() { Name = "Тервел" }, new() { Name = "Шабла" }
                ]
            },
            new()
            {
                Name = "Кърджали",
                Cities =
                [
                    new() { Name = "Кърджали" }, new() { Name = "Ардино" }, new() { Name = "Джебел" },
                    new() { Name = "Крумовград" }, new() { Name = "Момчилград" }
                ]
            },
            new()
            {
                Name = "Кюстендил",
                Cities =
                [
                    new() { Name = "Кюстендил" }, new() { Name = "Бобов дол" }, new() { Name = "Бобошево" },
                    new() { Name = "Дупница" }, new() { Name = "Кочериново" }, new() { Name = "Рила" },
                    new() { Name = "Сапарева баня" }
                ]
            },
            new()
            {
                Name = "Ловеч",
                Cities =
                [
                    new() { Name = "Ловеч" }, new() { Name = "Априлци" }, new() { Name = "Летница" },
                    new() { Name = "Луковит" }, new() { Name = "Тетевен" }, new() { Name = "Троян" },
                    new() { Name = "Угърчин" }, new() { Name = "Ябланица" }
                ]
            },
            new()
            {
                Name = "Монтана",
                Cities =
                [
                    new() { Name = "Монтана" }, new() { Name = "Берковица" }, new() { Name = "Бойчиновци" },
                    new() { Name = "Брусарци" }, new() { Name = "Вълчедръм" }, new() { Name = "Вършец" },
                    new() { Name = "Лом" }, new() { Name = "Чипровци" }
                ]
            },
            new()
            {
                Name = "Пазарджик",
                Cities =
                [
                    new() { Name = "Пазарджик" }, new() { Name = "Батак" }, new() { Name = "Белово" },
                    new() { Name = "Брацигово" }, new() { Name = "Велинград" }, new() { Name = "Ветрен" },
                    new() { Name = "Костандово" }, new() { Name = "Панагюрище" }, new() { Name = "Пещера" },
                    new() { Name = "Ракитово" }, new() { Name = "Септември" }, new() { Name = "Стрелча" },
                    new() { Name = "Сърница" }
                ]
            },
            new()
            {
                Name = "Перник",
                Cities =
                [
                    new() { Name = "Перник" }, new() { Name = "Батановци" }, new() { Name = "Брезник" },
                    new() { Name = "Земен" }, new() { Name = "Радомир" }, new() { Name = "Трън" }
                ]
            },
            new()
            {
                Name = "Плевен",
                Cities =
                [
                    new() { Name = "Плевен" }, new() { Name = "Белене" }, new() { Name = "Гулянци" },
                    new() { Name = "Долна Митрополия" }, new() { Name = "Долни Дъбник" }, new() { Name = "Искър" },
                    new() { Name = "Кнежа" }, new() { Name = "Койнаре" }, new() { Name = "Левски" },
                    new() { Name = "Никопол" }, new() { Name = "Пордим" }, new() { Name = "Славяново" },
                    new() { Name = "Тръстеник" }
                ]
            },
            new()
            {
                Name = "Пловдив",
                Cities =
                [
                    new() { Name = "Пловдив" }, new() { Name = "Асеновград" }, new() { Name = "Баня" },
                    new() { Name = "Брезово" }, new() { Name = "Калофер" }, new() { Name = "Карлово" },
                    new() { Name = "Клисура" }, new() { Name = "Кричим" }, new() { Name = "Куклен" },
                    new() { Name = "Лъки" }, new() { Name = "Първомай" }, new() { Name = "Раковски" },
                    new() { Name = "Садово" }, new() { Name = "Сопот" }, new() { Name = "Стамболийски" },
                    new() { Name = "Съединение" }, new() { Name = "Хисаря" }
                ]
            },
            new()
            {
                Name = "Разград",
                Cities =
                [
                    new() { Name = "Разград" }, new() { Name = "Завет" }, new() { Name = "Исперих" },
                    new() { Name = "Кубрат" }, new() { Name = "Лозница" }, new() { Name = "Цар Калоян" }
                ]
            },
            new()
            {
                Name = "Русе",
                Cities =
                [
                    new() { Name = "Русе" }, new() { Name = "Борово" }, new() { Name = "Бяла" },
                    new() { Name = "Ветово" }, new() { Name = "Глоджево" }, new() { Name = "Две могили" },
                    new() { Name = "Мартен" }, new() { Name = "Сеново" }, new() { Name = "Сливо поле" }
                ]
            },
            new()
            {
                Name = "Силистра",
                Cities =
                [
                    new() { Name = "Силистра" }, new() { Name = "Алфатар" }, new() { Name = "Главиница" },
                    new() { Name = "Дулово" }, new() { Name = "Тутракан" }
                ]
            },
            new()
            {
                Name = "Сливен",
                Cities =
                [
                    new() { Name = "Сливен" }, new() { Name = "Кермен" }, new() { Name = "Котел" },
                    new() { Name = "Нова Загора" }, new() { Name = "Твърдица" }, new() { Name = "Шивачево" }
                ]
            },
            new()
            {
                Name = "Смолян",
                Cities =
                [
                    new() { Name = "Смолян" }, new() { Name = "Девин" }, new() { Name = "Доспат" },
                    new() { Name = "Златоград" }, new() { Name = "Мадан" }, new() { Name = "Неделино" },
                    new() { Name = "Рудозем" }, new() { Name = "Чепеларе" }
                ]
            },
            new()
            {
                Name = "София (град)",
                Cities =
                [
                    new() { Name = "София" }, new() { Name = "Банкя" }, new() { Name = "Бухово" },
                    new() { Name = "Нови Искър" }
                ]
            },
            new()
            {
                Name = "Софийска област",
                Cities =
                [
                    new() { Name = "Божурище" }, new() { Name = "Ботевград" }, new() { Name = "Годеч" },
                    new() { Name = "Долна баня" }, new() { Name = "Драгоман" }, new() { Name = "Елин Пелин" },
                    new() { Name = "Етрополе" }, new() { Name = "Златица" }, new() { Name = "Ихтиман" },
                    new() { Name = "Копривщица" }, new() { Name = "Костенец" }, new() { Name = "Костинброд" },
                    new() { Name = "Правец" }, new() { Name = "Самоков" }, new() { Name = "Своге" },
                    new() { Name = "Сливница" }, new() { Name = "Пирдоп" }
                ]
            },
            new()
            {
                Name = "Стара Загора",
                Cities =
                [
                    new() { Name = "Стара Загора" }, new() { Name = "Гурково" }, new() { Name = "Гълъбово" },
                    new() { Name = "Казанлък" }, new() { Name = "Мъглиж" }, new() { Name = "Николаево" },
                    new() { Name = "Павел баня" }, new() { Name = "Раднево" }, new() { Name = "Чирпан" },
                    new() { Name = "Шипка" }
                ]
            },
            new()
            {
                Name = "Търговище",
                Cities =
                [
                    new() { Name = "Търговище" }, new() { Name = "Антоново" }, new() { Name = "Омуртаг" },
                    new() { Name = "Опака" }, new() { Name = "Попово" }
                ]
            },
            new()
            {
                Name = "Хасково",
                Cities =
                [
                    new() { Name = "Хасково" }, new() { Name = "Димитровград" }, new() { Name = "Ивайловград" },
                    new() { Name = "Любимец" }, new() { Name = "Маджарово" }, new() { Name = "Меричлери" },
                    new() { Name = "Свиленград" }, new() { Name = "Симеоновград" }, new() { Name = "Тополовград" },
                    new() { Name = "Харманли" }
                ]
            },
            new()
            {
                Name = "Шумен",
                Cities =
                [
                    new() { Name = "Шумен" }, new() { Name = "Велики Преслав" }, new() { Name = "Върбица" },
                    new() { Name = "Каолиново" }, new() { Name = "Каспичан" }, new() { Name = "Нови пазар" },
                    new() { Name = "Плиска" }, new() { Name = "Смядово" }
                ]
            },
            new()
            {
                Name = "Ямбол",
                Cities =
                [
                    new() { Name = "Ямбол" }, new() { Name = "Болярово" }, new() { Name = "Елхово" },
                    new() { Name = "Стралджа" }
                ]
            }
        };

        await context.Regions.AddRangeAsync(regions);
        await context.SaveChangesAsync();
    }
}
