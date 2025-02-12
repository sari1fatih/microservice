# .Net 8 - Elasticsearch - Kibana - Ocelot - RabbitMq - Hangfire

https://www.youtube.com/watch?v=qiFnLOjG_2s

The structure is being established...
# Prerequisites
.NET 8.0 Runtime


  ## Technologies used:

* ASP.NET Core
* Entity Framework Core*
* Swagger
* Mapper
* Serilog
* Rate Limit
* Ocelot
* Elasticsearch
* Kibana
* Hangfire
* RabbitMQ

Bu proje, kullanıcı yönetimi, müşteri yönetimi ve satış takibi için mikroservis mimarisiyle tasarlanmış bir sistemdir. Aşağıda projede yer alan beklenen özellikler ve değerlendirme kriterleri yer almaktadır.

## Beklenen Özellikler

### 1. Kullanıcı Yönetimi:
- Kullanıcılar sisteme giriş yapabilmelidir (JWT ile kimlik doğrulama).
- Kullanıcı rollerini yönetebilirsiniz (Admin, Satış Temsilcisi vb.).
- Kullanıcı bilgileri CRUD işlemleri ile yönetilebilir.

### 2. Müşteri Yönetimi:
- Müşteri bilgileri (isim, e-posta, telefon, şirket) CRUD işlemleri ile yönetilebilir.
- Müşterilere notlar eklenebilir ve düzenlenebilir.
- Müşteri listesi sıralanabilir ve filtrelenebilir.

### 3. Satış Takibi:
- Potansiyel satışlar için bir pipeline oluşturulabilir (örneğin: "Yeni", "İletişimde", "Anlaşma", "Kapandı").
- Her bir satış durumu için tarih ve notlar tutulabilir.
- Satış durumu değiştirilirken işlem tarihi kaydedilir.

### 4. Mikroservis Mimari:
- Kullanıcı yönetimi, müşteri yönetimi ve satış yönetimi ayrı mikro servisler olarak tasarlanmalıdır.
- Mikroservisler, bir API Gateway üzerinden haberleşmelidir.

### 5. Veri Tabanı:
- SQL ve NoSQL veritabanı tercihi ile veritabanı tasarımı yapılmalıdır.
- Her bir mikroservis kendi veritabanına sahip olmalıdır.

### 6. Performans ve Kullanılabilirlik:
- API çağrılarının performansı ve yanıt süreleri.

## Proje Teslim Şekli

- Mikroservis, versiyon kontrol sistemi ile (Git) versiyonlandırılıp uzak repoya (GitHub, GitLab, Bitbucket vb.) yüklenmelidir.
- Mikroservisi bir container içine alınız.
- Her mikroservis için ayrı bir klasör oluşturulmalıdır.
- Docker Compose ile tüm sistemi çalıştırmak için bir `docker-compose.yml` dosyası hazırlanmalıdır.
- Mikroservis yazılırken “clean code” ve güvenli kod yazma kurallarına özen gösterilmelidir.
- API dokümantasyonu Swagger üzerinden erişilebilir olmalıdır.
- Yaptığınız çalışmayı açıklayan bir video hazırlanıp linkinin gönderilmesi tercih edilir.
