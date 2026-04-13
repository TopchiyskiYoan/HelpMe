using HelpMe.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Infrastructure.Data;

public static class DbSeeder
{
    private const string DefaultPassword = "Test1234!";

    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager);
        await SeedCategoriesAsync(context);
        await SeedRegionsAndCitiesAsync(context);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = ["Administrator", "Client", "Handyman"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        await CreateUserIfNotExists(userManager, new ApplicationUser
        {
            UserName = "admin@helpme.bg",
            Email = "admin@helpme.bg",
            FirstName = "Admin",
            LastName = "HelpMe",
            PhoneNumber = "0888000001",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        }, "Administrator");

        await CreateUserIfNotExists(userManager, new ApplicationUser
        {
            UserName = "client@helpme.bg",
            Email = "client@helpme.bg",
            FirstName = "Georgi",
            LastName = "Petrov",
            PhoneNumber = "0888000002",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        }, "Client");

        await CreateUserIfNotExists(userManager, new ApplicationUser
        {
            UserName = "handyman@helpme.bg",
            Email = "handyman@helpme.bg",
            FirstName = "Dimitar",
            LastName = "Kolev",
            PhoneNumber = "0888000003",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        }, "Handyman");
    }

    private static async Task CreateUserIfNotExists(
        UserManager<ApplicationUser> userManager,
        ApplicationUser user,
        string role)
    {
        if (await userManager.FindByEmailAsync(user.Email!) is not null)
            return;

        await userManager.CreateAsync(user, DefaultPassword);
        await userManager.AddToRoleAsync(user, role);
    }

    private static async Task SeedCategoriesAsync(ApplicationDbContext context)
    {
        if (await context.ServiceCategories.AnyAsync())
            return;

        var categories = new List<ServiceCategory>
        {
            new()
            {
                Name = "Къртене и демонтаж",
                SubCategories =
                [
                    new() { Name = "Къртене бетон", Description = "Къртене на бетонни елементи и конструкции" },
                    new() { Name = "Къртене тухла", Description = "Разрушаване на неносещи тухлени стени" },
                    new() { Name = "Демонтаж дограма", Description = "Демонтаж на стара дограма и подпрозоречни первази" },
                    new() { Name = "Премахване настилки", Description = "Къртене на стари плочки, замазки и мозайки" },
                    new() { Name = "Чистене и извозване", Description = "Събиране на строителни отпадъци в чували и извозване до сметище" }
                ]
            },
            new()
            {
                Name = "Електро услуги",
                SubCategories =
                [
                    new() { Name = "Нова инсталация", Description = "Цялостно проектиране и изграждане на електрически инсталации" },
                    new() { Name = "Ел. табла", Description = "Монтаж и окабеляване на разпределителни ел. табла" },
                    new() { Name = "Ключове и контакти", Description = "Монтаж на интериорни ключове, контакти и розетки" },
                    new() { Name = "Осветление", Description = "Монтаж на полилеи, лунички и LED ленти" },
                    new() { Name = "Слаботокови инсталации", Description = "Изграждане на интернет, TV и домофонни системи" }
                ]
            },
            new()
            {
                Name = "ВиК услуги",
                SubCategories =
                [
                    new() { Name = "Водопровод и канализация", Description = "Изграждане на нови водопроводни и канализационни мрежи" },
                    new() { Name = "Монтаж санитария", Description = "Монтаж на мивки, тоалетни чинии, бидета и смесители" },
                    new() { Name = "Вертикални щрангове", Description = "Смяна на вертикални и хоризонтални щрангове" },
                    new() { Name = "Течове", Description = "Диагностика с термокамера и отстраняване на течове" },
                    new() { Name = "Свързване на уреди", Description = "Монтаж и свързване на перални, съдомиялни и бойлери" }
                ]
            },
            new()
            {
                Name = "Сухо строителство",
                SubCategories =
                [
                    new() { Name = "Окачени тавани", Description = "Монтаж на окачени тавани от гипсокартон на конструкция" },
                    new() { Name = "Преградни стени", Description = "Изграждане на преградни стени от гипсокартон" },
                    new() { Name = "Предстенни обшивки", Description = "Монтаж на гипсокартон върху съществуващи стени" },
                    new() { Name = "Растерни тавани", Description = "Монтаж на растерни тавани за офиси и търговски площи" },
                    new() { Name = "Вата и изолация", Description = "Полагане на минерална или каменна вата за шумо- и топлоизолация" }
                ]
            },
            new()
            {
                Name = "Грубо строителство и мазилки",
                SubCategories =
                [
                    new() { Name = "Замазки", Description = "Полагане на пясъчно-циментови и саморазливни замазки" },
                    new() { Name = "Машинна мазилка", Description = "Нанасяне на гипсова или варо-циментова машинна мазилка" },
                    new() { Name = "Ръчна мазилка", Description = "Нанасяне на хастар и варова мазилка на ръка" },
                    new() { Name = "Обръщане на прозорци", Description = "Подмазване и изправяне на ъгли след смяна на дограма" }
                ]
            },
            new()
            {
                Name = "Шпакловка и боядисване",
                SubCategories =
                [
                    new() { Name = "Гипсова шпакловка", Description = "Цялостна шпакловка на стени и тавани за изравняване" },
                    new() { Name = "Фино боядисване", Description = "Боядисване с интериорен латекс, акрилни бои или фасаген" },
                    new() { Name = "Декоративни мазилки", Description = "Полагане на венецианска, стуко, травертино и други ефектни мазилки" },
                    new() { Name = "Тапети", Description = "Грундиране и лепене на хартиени, флис или винилови тапети" },
                    new() { Name = "Шлайфане", Description = "Машинно шлайфане на повърхности преди боядисване" }
                ]
            },
            new()
            {
                Name = "Облицовки и настилки",
                SubCategories =
                [
                    new() { Name = "Плочки", Description = "Лепене на фаянс, теракота, гранитогрес и камък" },
                    new() { Name = "Паркет", Description = "Монтаж на ламиниран, трислоен и многослоен паркет" },
                    new() { Name = "Циклене паркет", Description = "Машинно циклене, фугиране и лакиране на естествен паркет" },
                    new() { Name = "Первази", Description = "Монтаж на PVC, дървени или MDF подови первази" },
                    new() { Name = "Хидроизолация баня", Description = "Полагане на течна хидроизолация и ленти в мокри помещения" }
                ]
            },
            new()
            {
                Name = "Отопление, Вентилация и Климатизация",
                SubCategories =
                [
                    new() { Name = "Подово отопление", Description = "Монтаж на водно или електрическо подово отопление" },
                    new() { Name = "Радиатори", Description = "Монтаж, демонтаж и преместване на отоплителни тела" },
                    new() { Name = "Климатици", Description = "Монтаж и пускане в експлоатация на климатични системи" },
                    new() { Name = "Вентилация", Description = "Изграждане на въздуховодни пътища и монтаж на вентилатори" }
                ]
            },
            new()
            {
                Name = "Довършителни работи",
                SubCategories =
                [
                    new() { Name = "Мебели монтаж", Description = "Сглобяване и нивелиране на корпусни и меки мебели" },
                    new() { Name = "Врати монтаж", Description = "Монтаж на интериорни и блиндирани входни врати" },
                    new() { Name = "Кухни монтаж", Description = "Монтаж на кухненски модули, вграждане на уреди и изрязване на плот" },
                    new() { Name = "Аксесоари баня", Description = "Монтаж на огледала, закачалки, паравани и душ кабини" }
                ]
            },
            new()
            {
                Name = "Изолация и Фасада",
                SubCategories =
                [
                    new() { Name = "Топлоизолация", Description = "Цялостна външна топлоизолационна система (EPS, XPS, вата)" },
                    new() { Name = "Фасадно боядисване", Description = "Почистване, грундиране и боядисване на външни стени" },
                    new() { Name = "Покривни ремонти", Description = "Ремонт на покриви, смяна на керемиди и улуци" }
                ]
            }
        };

        await context.ServiceCategories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }

    private static async Task SeedRegionsAndCitiesAsync(ApplicationDbContext context)
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
