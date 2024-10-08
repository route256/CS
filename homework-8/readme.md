## Домашнее задание 8 недели обучения

Надо добавить в сервисы логирование, трейсинг и метрики, а так же подключить грейлог, егеря и прометеус с графаной.

##### Задание на алмазик 💎
Самостоятельно пройтись по теме Domain-Oriented Observability (можно начать с https://martinfowler.com/articles/domain-oriented-observability.html), реализовать и покрыть тестами.

Дедлайе: 11 ноября, 23:59 (сдача) / 14 ноября, 23:59 (проверка)

---

## Домашнее задание седьмой недели обучения

Домашнее задание досточно творческое. Оон заключается в рефакторинге вашего сервиса и приведении его к стандартам чистого кода и чистой аритектуры.

1. Создать проект Domain, в него поместить доменную модель и вынести в нее всю необходимую бизнес-логику
2. Создать проект Infrastructure и перенести в него весь инфраструктурный код: работы с БД шардированием, ServiceDiscovery, Kafka и пр.
3. Сделать все классы в проекте Infrastructure с доступом internal.
4. Создать проект Application и реалзиовать в нем необходимые сервисы приложения
5. "Очистить" проект Host (текущий проект по-умолчанию), в нем должна остаться только логика gPRC сервиса, чтение из Кафки (если необходимо), маппинг.
6. *Примечание*: использовать MediatR допустимо, но не обязательно.

##### Задание со звездочкой
Создать классы для примитивных типов там где нужно и поместить в них необходимкю бизнес логику. Например, вместо `string Email { get; }` сделать класс `Email`/

Дедлайн: 4 ноября, 23:59 (сдача) / 7 ноября, 23:59 (проверка)

---

## Домашнее задание шестой недели обучения

1. Поднять в окружении 2 инстанса БД для order-service
2. Сконфигурировать service-discovery для новых инстансов (orders-1, buckets 0-3; orders-2, buckets 4-7)
3. Выбрать ключ(и) шардирования и реализовать функцию для определения бакета по ключу
4. Настроить connectionFactory на работу с шардированной базой через service discovery
5. Переделать мигратор на работу с шардированной базой
6. Выполнить миграции на шардированный кластер (по бакетам)
7. Перевести работу сервиса на шардированную базу

##### Задание со звездочкой
Реализовать индексный поиск для получения заказов не по ключу шардирования (согласно контрактам) (*).

### FAQ

##### Как быть с ручками, которые ищут не по ключу с пагинацией и т.п.?
Для выполнения задания достаточно, чтобы вы перебирали все заказы из всех бакетов. 
Но для ендпоинтов, осущетсвляющих поиск не по ключу шарирования, можно реализовать глобальный индекс (ради алмазика).
Подсказка - в индекс можно добавить необходимые поля для фильтрации

##### Как быть с другими сущностями в нашей БД — склады/регионы?
Так как это справочники, то их состав допустимо просто копировать во все шарды и держать в каждом шарде копию.
Вы можете шардировать также и их, но не забудьте снять foreign key constraint, если они у вас были. 

##### Как быть с транзакционностью?
Для выполнения задания с алмазиком при реализации глобального индекса НЕ требуется, чтобы его обновление было выполнено транзационно с 
вставкой/изменением основной сущности, т.к. в общем случае оно будет выполняться на другом шарде и потребует распределенной транзакции. 
При этом, следует сохранить транзакционность сохранения самих сущностей в рамках одного запроса.

##### Как быть с множественной вставкой?
Реализация вставки массива объектов остается на ваш выбор. 
Для использования контрукции `unnest` допускается создавать пользовательский тип в базе только в схеме public (через миграции).

### Критерии приемки:

1. Присутствуют базы данных orders-1 и orders-2
2. В базе orders-1 находятся схемы bucket_0, bucket_1, bucket_2, bucket_3
3. В базе orders-2 находятся схемы bucket_4, bucket_5, bucket_6, bucket_7
4. В каждую схему bucket_N смигрирована структура БД
5. Реализован IShardingRule, выполняющий преобразование ключа шардирования в номер бакета
6. Реализован ShardConnectionFactory, создающий подключения к нужному инстансу БД на основе ключа
7. Источником данных для определения распределения бакетов по хостам в ShardConnectionFactory должен быть ServiceDiscovery
8. Репозитории переведены на использование ShardConnectionFactory
9. Все API работают как и прежде.

#### Для задания со звездочкой:
1. Создана индексная таблица для поиска заказов не по ключу шардирования. Количество индексов опрделеяется контрактами на поиск заказов (*)
2. При вставке (обновлении, затраигивающем индексное поле) заказа, также заносится/обновляется запись в индексную таблицу/ы (без транзакции)
3. Поиск заказов не по ключу шардирования производится через индексную таблицу/ы, а не по всем бакетам

**(*)** Для выполнения задания со звездочкой, достаточно реализовать индекс по регионам и использовать его для всех подходящих запросов. 
Использование индекса для поиска заказов по клиенту остается на ваше усмотрение.

Дедлайн: 28 октября, 23:59 (сдача) / 31 октября, 23:59 (проверка)

---

## Домашнее задание пятой недели обучения

### Спроектировать базу данных orders для хранения заказов

* Обеспечить хранение данных заказа, регионов (совместно со складом или отдельно склады). Тип полей выбирать исходя из полей ваших моделей данных. Допустимо использовать jsonb/enum в случае необходимости. Товарный состав заказа сохранять не нужно.

* Создать слой миграций используя FluentMigrator. Миграция должна быть строго SQL, fluent-синтаксис недопустим. Миграция должна создавать БД и необходимые индексы. Необходимость индекса определяется самостоятельно.

* Реализовать интерфейсы репозиториев (существующие in-memory оставить). Все методы абстрактных репозиториев должны быть асинхронными (возвращать `Task<>`) и потдерживать отмену (`CancellationToken`)

* Удалять ранее реализованные методы API и/или методы репозиториев тоже нельзя.

Задание со звездочкой:
* Написать интеграционные тесты для репозиториев или целиком сервиса через gRPC API. Обеспечивать запуск интеграционных тестов в ci/cd не
  нужно.

Дедлайн: 21 октября, 23:59 (сдача) / 24 октября, 23:59 (проверка)

---

## Домашнее задание четвертой недели обучения

#### 1. Необходимо кэшировать ответы от сервиса CustomerService

* Реализовать кэширование через `IDistributedCache` или `StackExchange.Redis` (На воршкопе расматривался вариант только с StackExchange.Redis)

#### 2. Необходимо реализовать Consumer для топика pre_orders
* Получаем данные из топика `pre_orders`
* Обогащаем данными из сервиса CustomerService
* Надо обогатить данные так, что мы могли выполнять агрегацию данных
* Обогащенные данные сохраняем в репозиторий

#### 3. Валидация данных перед отправкой в new_orders
* Добавить кадому региону склад с координатами
* Координаты придумайте сами
* Проверяем расстояние между адресом в заказе и складом региона
* Если расстояние более 5000, то заказ не валидиный
  ** Там захардкожены только двое координат. (55.7522, 37.6156 и 55.01, 82.55)

* Заказы сохраняются независимо от валидности

#### 4. Необходимо реализовать Poducer для топика new_orders
* Валидные заказы необходимо отправлять в топик `new_orders`

#### 5. Необходимо реализовать Consumer для топика orders_events
* Читать сообщения из топика `orders_events`
* Обновлять статус заказа

** Контракт для топика `pre_orders`
key:orderId
value:
```json
{
    "Id": 82788613,
    "Source": 1,
    "Customer": {
        "Id": 1333768,
        "Address": {
            "Region": "Montana",
            "City": "East Erich",
            "Street": "Bernier Stream",
            "Building": "0744",
            "Apartment": "447",
            "Latitude": -29.8206,
            "Longitude": -50.1263
        }
    },
    "Goods": [
        {
            "Id": 5140271,
            "Name": "Intelligent Rubber Shoes",
            "Quantity": 6,
            "Price": 2204.92,
            "Weight": 2802271506
        },
        {
            "Id": 2594594,
            "Name": "Rustic Frozen Pants",
            "Quantity": 8,
            "Price": 1576.55,
            "Weight": 3174423838
        },
        {
            "Id": 6005559,
            "Name": "Practical Plastic Soap",
            "Quantity": 2,
            "Price": 1034.51,
            "Weight": 2587375422
        }
    ]
}
```

** Контракт для топика `new_orders`
key:orderId
value:
```json
{"OrderId": 1}
```

** Контракт для топика `orders_events`
key:orderId
value:
```json
{
	"Id": 20032,
	"NewState": "SentToCustomer",
    "UpdateDate": "2023-03-11T11:40:44.964164+00:00"
}
```

Задание со **\***

* Написать Unit тесты для новой логики

> **Дедлайн: 17 июня 23:59 (сдача) / 20 июня 23:59 (проверка).**

---

## Домашнее задание 3

### 1. Необходимо реализовать хранилища данных в сервисе OrderService по аналогии с тем, как это сделано было на workshop.

* Необходимо реализовать абстрактные репозитории (интерфейсы)
* Все методы абстрактных репозиториев должны быть асинхронными(возвращать `Task<>`) и потдерживать отмену (`CancellationToken`)

* Реализация репозиториев должна основываться на базе конкурентного словаря (`ConcurrentDictionary<,>`)
* Реализация репозиториев должна потдерживать отмену (`CancellationToken`)

* Список регионов: "Moscow", "StPetersburg", "Novosibirsk"

### 2. Необходимо реализовать GRPC клиент для сервиса LogisticsSimulator
* [Proto file](https://gitlab.ozon.dev/cs/classroom-8/students/practice/-/blob/master/src/Ozon.Route256.Practice.LogisticsSimulator/Proto/LogisticsSimulator.proto)

### 3. Написать бизнес логику для GRPC ручек
* Убрать все заглушки
* Связать бизнес логику с репозиториями
* При реализации метода "отмены заказа", помимо отмены у себя в хранилище, надо отменить заказ в LogisticsSimulator через GRPC

Задание со *

* Для внутренних сервисов написаны UnitTest. Написать (переписать) тесты с учетом репозитория. Тесты для самого репозитория делать не надо. Во внутренних сервисах абстрактный репозиторий следует замокать FakeItEasy / moq / и т.п.

Дедлайн: 7 октября, 23:59 (сдача) / 10 октября, 23:59 (проверка)

---

## Домашнее задание 2

### 1. Необходимо реализовать взаимодействие с SD в сервисе OrderService.
Данные из SD нужны буду на 6 неделе, когда будете проходить шардированные БД.

### 2. Необходимо реализовать ручки GRPC сервиса в OrdersService (название сервиса по желанию)

- Ручка отмены заказа
- Ручка возврата статуса заказа
- Ручка возврата списка регионов
- Ручка возврата списка заказов
- Ручка агрегации заказов по региону
- Ручка получения всех заказов клиента

#### GRPC API

- GRPC ручки должны описывать только контракты для взаимодействия. Реализация этих ручек не требуется.
- Все ручки, которые могут вернуть пустой список - должны возвращать пустой список.
- Если ручка может вернуть код NotFound - сейчас она должна возвращать именно этот код.
- Допускается добавить начальные валидации на входящие параметры и возвращать код 400 для REST запросов или StatusCode.InvalidArgument, если параметры не прошли валидацию.

### 3. Необходимо реализовать ручки для REST API в сервисе Gateway - все ручки в Gateway проксируют вызовы в соответствующие вызовы в grpc сервисы OrderService и CustomerService

- Ручка отмены заказа
- Ручка возврата статуса заказа
- Ручка возврата списка клиентов
- Ручка возврата списка регионов
- Ручка возврата списка заказов
- Ручка агрегации заказов по региону
- Ручка получения всех заказов клиента

#### REST API:

- Ручки должны отображаться в swagger.
- Количество контроллеров и способ их реализации определяется вами исходя из лучших практик, которые вам известны
- Должны вызывать соответствующие методы в сервисах OrderService и CustomerService, т.е. преобразовать REST запрос в GRPC запрос, сделать вызов, получить GRPC ответ или ошибку и преобразовать его в REST ответ.
- Кроме проксирования запросов - никаких другой логики в них быть не должно.
- Клиент для OrderService должен уметь балансировать нагрузку на разные экземпляры этого сервиса (пример как это сделать был в воркшопе).
- replicas в docker-compose использовать не нужно. Все реплики должны быть прописаны вручную.

Не допускается использовать сторонние библиотеки, кроме основного фреймворка и тех, что были задействованы на workshop.

#### Подробное поведение всех ручек описано https://hallowed-join-b37.notion.site/9a297c7d74ae4b8a93feae6d8af36531

### 5. Дополнительное задание на алмазик
- В сваггере выводится вся информация об ошибках, описание контракта ответа в случае кодов 200, 400, 500
- Для внутренних сервисов написаны UnitTest, например для классов работающих с SD, или для классов, которые выполняют дополнительные проверки (валидации). Для ручек писать тесты на данном этапе не требуется.
- Для UnitTest использовать xUnit

### 6. Контракты возвращаемых объектов:

#### Для отмены заказа:

* Статус отмены заказа - успешно отменено или нет
* Описание причны не удавшейся отмены, если отменить заказ не удалось. Если Омена успешна - поле пустое.

#### Для ручки агрегации:

* Название региона
* Количество заказов
* Общая сумма заказов
* Суммарный вес
* Количество клиентов, сделавших заказ в этом регионе

#### Данные по заказу для всех ручек.

* Id заказа
* Количество товаров в заказе
* Общая сумма заказа
* Общий вес заказа
* Тип заказа
* Дата заказа
* Откуда сделан заказа (регион)
* Статус заказа
* Имя клиента (имя и фамилия)
* Адрес доставки
* Телефон

---

## Домашнее задание 1
### Развернуть инфраструктуру сервисов

### Требуется:
- Создать репозиторий проекта в своем личном пространстве нашего гитлаба.
- Название проекта в гитлабе по вашему усмотрению.
- Дать доступ к проекту своему тьютору.
- Создать решение Ozon.Route256.Practice

### Состав решения
- Проект Ozon.Route256.Practice.OrdersService
- Проект Ozon.Route256.Practice.GatewayService
- Для обоих проектов должны быть описаны Dockerfile
- Описать файл docker-compose.yaml

### Состав docker-compose.yaml
- Сервисы на базе образов из gitlab-registry.ozon.dev
  - CustomerService
  - LogisticsSimulator
  - OrdersGenerator - в трех экземплярах и представлять разные источники WebSite, Mobile, Api
  - ServiceDiscovery
- Сервисы на базе проектов в решении
  - OrdersService - в двух экземплярах
  - GatewayService
- postgress база данных для сервиса CustomerService (customer-service-db)
- postgress база данных для сервиса OrdersService (orders)
- Инфраструктура кафки (брокеры и zookeeper)
- OrdersService должно быть 2 экземпляра

### Закомитить проект

### Конечный результат:

Присутствуют images:
- CustomerService
- OrdersService - в двух экземплярах
- GatewayService
- LogisticsSimulator
- OrderGenerator - в трех экземплярах и представлять разные источники WebSite, Mobile, Api
- ServiceDiscovery
- kafka (broker)
- zookeeper
- postgres

Корректно стартуют контейнеры:
- CustomerService
- OrderService
- GatewayService
- LogisticsSimulator
- OrdersGenerator
- ServiceDiscovery
- kafka (broker)
- zookeeper
- postgres

p.s. По желанию можно добавить в решение файл .gitlab-ci.yml с настройками конкретно для вашего проекта, которые были представлены на воркшопе.

Дедлайн: 23 сентября 23:59 (сдача) / 25 сентября 23:59 (проверка) 
