# Ozon Route 256 — Postgres Example

### Docker
* Если у вас нет docker — можно поставить локально postgres. Но docker нам явно пригодится на следующих неделях
* Запустить docker контейнеры (БД): `docker compose up -d`
* Остановить docker контейнеры (БД): `docker compose down`
* Остановить и почистить docker от данных: `docker compose down -v`
* Docker поломался: `docker system prune`

### Приложение
* Чтобы наполнить БД данными — используйте интеграционный тест `GenerateDataTests.Generate`
* Используте профиль запуска `Migrate`, чтобы прокатить миграции (интеграционники катят их сами)
* Ипользуйте профиль запуска `Run`, чтобы запустить проект локально
* Используйте `grpcui` или `Postman`, чтобы подёргать gRPC API
* Используйте `Rider`, `DataGrip`, `dBeaver`, `pgAdmin`, `Visual Studio Code`, чтобы исполнять запросы к БД

### Домашнее задание
Создать gRPC API для получения заказов клиента (с товарами).
* Вход:
* * `clientId` *,
* * `pageSize` *,
* * `startFromOrderId` (пустое значение считаем, что хотим получать самые последние заказы)
* Выход: Все поля `orders` + `order_items`
* Сортировка: `order_id DESC, sku_id ASC`

Общие требования
* Использовать в репозитории `IAsyncEnumerable` (по аналогии с существующим методом `Get`)
* API должно быть быстрым (потребуется миграция с индексом)
* Написать интеграционный тест (по аналогии с существующим) для новой gRPC API.
* Бонус, если proto будет соответствовать Google Protobuf Style Guide: https://developers.google.com/protocol-buffers/docs/style
* Бонус, если будет написан benchmark для измерения скорости новой gRPC API (отдельным консольным приложением или интеграционным тестом). Однопоточный. Набор данных — перед запуском бенчмарка достать из базки 1000 случайных clientId, дернуть API 1000 раз (с каждым клиентом). Замерить минимальное, среднее, максимальное время.

### Домашнее задание для упорных, бесстрашных людей с кучей свободного времени
* Сделать домашнее задание выше
* Перейти в отдельную git-ветку
* Переделать схему данных под хранение товарной части заказа в виде `jsonb`-поля у таблицы `orders` вместо отдельной таблицы `order_items`
* Переделать проект под такую схему
* Побенчмаркать оба варианта
* Рассказать в чате результаты
* Вы великолепны