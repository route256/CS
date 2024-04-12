# Домашнее задание 1
# Развернуть инфраструктуру сервисов

## Требуется:
- Создать репозиторий проекта в своем личном пространстве нашего гитлаба.
- Название проекта в гитлабе по вашему усмотрению.
- Дать доступ к проекту своему тьютору.
- Создать решение Ozon.Route256.Practice

## Состав решения
- Проект Ozon.Route256.Practice.OrdersService
- Проект Ozon.Route256.Practice.GatewayService
- Для обоих проектов должны быть описаны Dockerfile
- Описать файл docker-compose.yaml

## Состав docker-compose.yaml
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

### Закомитить проект.

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

