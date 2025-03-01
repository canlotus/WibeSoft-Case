Wibesoft Case

Bu proje, Unity C# kullanılarak geliştirilmiş grid tabanlı bir çiftlik/simülasyon oyunudur. Projede, oyuncuların tarla üzerinde ekin ekmesi, ekinlerin büyüme ve hasat süreçlerinin yönetilmesi, binaların yerleştirilmesi ve yeniden konumlandırılması gibi temel işlevler yer almaktadır. Ayrıca, kullanıcı dostu UI/UX düzenlemeleri, envanter yönetimi, kaynak satın alma ve kamera sürükleme özellikleri de uygulanmıştır. Proje, mobil platformlara uygun şekilde tasarlanmıştır.

Özellikler
	•	Ekin Sistemi:
	•	Grid tabanlı tarla üzerinde ekin ekme, büyüme ve hasat mekanizması.
	•	En az iki farklı ekin türü (bu örnekte Wheat ve Corn) yer almaktadır.
	•	Ekinlerin büyüme aşamalarını temsil eden değişen sprite’lar.
	•	Ekim işlemi sonrasında ekinlerin hasat edilerek envantere eklenmesi.
	•	Bina Yerleştirme Mekanizması:
	•	Grid tabanlı araziye binaların yerleştirilmesi.
	•	Bina yerleştirme sırasında geçerli/geçersiz alanların görsel olarak belirtilmesi.
	•	Binaların yeniden konumlandırılabilmesi (taşıma modu).
	•	Taşıma işlemi tamamlandığında, eski konumun çimen (grass tile) ile güncellenmesi ve yeni konumun PlayerPrefs ile kalıcı hale getirilmesi.
	•	UI/UX ve Diğer Özellikler:
	•	Global büyüme paneli üzerinden crop (ekin) büyüme durumunun izlenmesi.
	•	Shop paneli üzerinden oyuncu kaynaklarının (tohum, altın) satın alınması.
	•	Envanter yönetimi: Hasat sonrası ürünlerin saklanması ve görüntülenmesi.
	•	Kamera sürükleme (drag) özelliği ile oyunun haritası üzerinde serbest gezinme.
	•	Mod Toggle: Oyuncunun bina (düzenleme) ve crop modları arasında geçiş yapmasını sağlayan UI butonları.

Kullanılan Scriptler

TilemapClickHandler.cs
	•	Amacı:
Kullanıcı tıklamalarını algılayarak tilemap üzerindeki tile’ların durumuna göre ilgili işlemleri (ekin panelinin açılması veya bina yerleştirme işlemlerinin başlatılması) tetikler.
	•	Özellikler:
	•	Building mode aktifken, bina tile’larına tıklanması durumunda taşımaya yönelik move butonunu aktif eder.
	•	Bina modu kapalı ise, boş tile’lara tıklanarak crop (ekin) panelinin açılmasını sağlar.

TileConversionManager.cs
	•	Amacı:
Tile’ların bina veya ekim işlemleri için dönüştürülmesini yönetir.
	•	Özellikler:
	•	Dönüşüm maliyetlerini hesaplar, altın miktarını günceller.
	•	Bina taşımaya yönelik move modunu yönetir; taşınan binanın eski konumunu çimen tile ile günceller.
	•	PlayerPrefs kullanılarak tilemap durumunu kalıcı hale getirir.

CropSelectionManager.cs
	•	Amacı:
Çiftlik tarlası üzerinde ekin ekme işlemlerini kontrol eder.
	•	Özellikler:
	•	Crop (ekin) seçim panelini açar.
	•	Wheat ve Corn gibi ekinlerin ekilmesini sağlar.
	•	Ekim sonrası tile durumunu PlayerPrefs ile günceller.

CropGrowth.cs
	•	Amacı:
Ekilen ürünlerin büyüme sürecini yönetir.
	•	Özellikler:
	•	Ekinlerin büyüme aşamalarını (stage1, stage2, stage3) sprite’lar aracılığıyla görselleştirir.
	•	Büyüme tamamlandığında hasat işlemini başlatır.
	•	Büyüme durumunu global UI paneli üzerinden görüntüler ve PlayerPrefs ile kalıcı hale getirir.

ShopPanelManager.cs
	•	Amacı:
Oyuncunun tohum (wheat seed, corn seed) satın alması için bir mağaza paneli sağlar.
	•	Özellikler:
	•	Altın miktarını ve tohum fiyatlarını gösterir.
	•	Satın alma işlemleri sonrası oyuncu verilerini PlayerPrefs ile günceller.
	•	UI güncellemeleriyle, mevcut tohum ve altın miktarını ekrana yansıtır.

UIManager.cs
	•	Amacı:
Global UI öğelerine erişimi kolaylaştırmak için singleton yapıda çalışır.
	•	Özellikler:
	•	Global crop growth panelinin referansını tutar.
	•	Diğer UI scriptleri tarafından erişilebilmesi için merkezi bir yönetim sağlar.

InventoryManager.cs
	•	Amacı:
Oyuncunun hasat ettiği ürünleri saklamak ve görüntülemek için envanter yönetimi sağlar.
	•	Özellikler:
	•	Hasat sonrası ürünleri envantere ekler ve UI üzerinden gösterir.
	•	Oyuncunun envanter verilerini PlayerPrefs ile saklar.
	•	Singleton yapıda çalışır.

ModeToggleManager.cs
	•	Amacı:
Oyuncunun bina (düzenleme) ve crop modları arasında geçiş yapabilmesini sağlar.
	•	Özellikler:
	•	İki farklı UI butonu aracılığıyla modların aktif edilmesini sağlar.
	•	Aktif modun butonunu disable, diğer modun butonunu enable eder.

CameraDrag.cs
	•	Amacı:
Kameranın oyun sahnesinde mouse sürükleme yöntemiyle hareket etmesini sağlar.
	•	Özellikler:
	•	Kameranın hareketini belirli sınırlar (minX, maxX, minY, maxY) dahilinde kısıtlar.
	•	Mouse drag işlemiyle kamera pozisyonunu günceller.

Nasıl Çalıştırılır
	1.	Depoyu Klonlayın:
GitHub’daki public repo’yu klonlayın veya zip dosyasını indirin.
	2.	Unity Projesi Olarak Açın:
Unity Hub üzerinden projeyi açın. Projede kullanılan Unity sürümü README içinde belirtilmiştir.
	3.	Sahne ve Asset Ayarları:
İlgili sahneler açıldığında, script referanslarının Inspector üzerinden doğru şekilde atandığından emin olun (örneğin, UI öğeleri, tile referansları vb.).
	4.	Oyun Modu:
Play Mode’a geçerek oyunu test edebilir, crop ekme, bina yerleştirme, envanter ve mağaza gibi özellikleri deneyimleyebilirsiniz.

Kullanılan Teknolojiler ve Sistem Mimarisi
	•	Unity 3D:
Oyun motoru olarak Unity kullanılmıştır.
	•	C#:
Scriptlerin geliştirilmesinde C# dili tercih edilmiştir.
	•	PlayerPrefs:
Oyuncu verileri, tilemap durumu, envanter ve tohum sayıları gibi veriler için kalıcılık sağlamak amacıyla kullanılmıştır.
	•	UI/UX:
Unity’nin UI sistemi ve TextMeshPro kullanılarak, kullanıcı arayüzü oluşturulmuştur.
	•	Mobil Uyumluluk:
Proje, mobil platformlarda çalışacak şekilde optimize edilmiştir.
