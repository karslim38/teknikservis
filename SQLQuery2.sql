/* ============================================================
   TEKNIK SERVIS OTOMASYONU - VERITABANI KURULUMU
   Dikkat: Varsa eski TeknikServis veritabanini SILER ve bastan kurar.
   ============================================================ */

USE master;
GO

IF DB_ID(N'TeknikServis') IS NOT NULL
BEGIN
    ALTER DATABASE TeknikServis SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TeknikServis;
END
GO

CREATE DATABASE TeknikServis;
GO

USE TeknikServis;
GO

/* ---------- 1) YETKILENDIRME ---------- */
CREATE TABLE Yetkilendirme (
    yetkiid         INT IDENTITY(1,1) PRIMARY KEY,
    YetkiAdi        NVARCHAR(50) NOT NULL,
    PersonelEkleme  BIT NOT NULL DEFAULT 0,
    PersonelSilme   BIT NOT NULL DEFAULT 0,
    KayitAcma       BIT NOT NULL DEFAULT 0,
    KayitTamamlama  BIT NOT NULL DEFAULT 0,
    YedekParca      BIT NOT NULL DEFAULT 0,
    Carilislemleri  BIT NOT NULL DEFAULT 0,
    SistemAyarlari  BIT NOT NULL DEFAULT 0
);
GO

/* ---------- 2) PERSONEL ---------- */
CREATE TABLE Personel (
    PersonelNo   INT IDENTITY(1,1) PRIMARY KEY,
    TC           NVARCHAR(11) NOT NULL UNIQUE,      -- ayni TC iki kez girilemez
    KullaniciAdi NVARCHAR(50) NOT NULL,
    AdiSoyadi    NVARCHAR(100) NOT NULL,
    Telefon      NVARCHAR(15),
    Mail         NVARCHAR(100),
    Adres        NVARCHAR(250),
    Gorevi       NVARCHAR(50),
    Yetkisi      INT NOT NULL
                 CONSTRAINT FK_Personel_Yetki FOREIGN KEY REFERENCES Yetkilendirme(yetkiid),
    Maas         DECIMAL(10,2) NOT NULL DEFAULT 0,
    Sifre        NVARCHAR(100) NOT NULL,            -- sisteme giris sifresi
    Durum        BIT NOT NULL DEFAULT 1             -- 1 aktif, 0 pasif (silmek yerine)
);
GO

/* ---------- 3) CIHAZLAR ---------- */
CREATE TABLE cihazlar (
    cihazid   INT IDENTITY(1,1) PRIMARY KEY,
    CihazAdi  NVARCHAR(50) NOT NULL
);
GO

/* ---------- 4) MARKA ---------- */
CREATE TABLE Marka (
    Markaid   INT IDENTITY(1,1) PRIMARY KEY,
    MarkaAdi  NVARCHAR(50) NOT NULL
);
GO

/* ---------- 5) YEDEK PARCA KATEGORI ---------- */
CREATE TABLE YedekParcaKategori (
    KategoriId   INT IDENTITY(1,1) PRIMARY KEY,
    KategoriAdi  NVARCHAR(50) NOT NULL
);
GO

/* ---------- 6) YEDEK PARCA ---------- */
CREATE TABLE YedekParca (
    YedekParcaId  INT IDENTITY(1,1) PRIMARY KEY,
    Kategori      INT NOT NULL
                  CONSTRAINT FK_YedekParca_Kategori FOREIGN KEY REFERENCES YedekParcaKategori(KategoriId),
    Marka         INT NOT NULL
                  CONSTRAINT FK_YedekParca_Marka FOREIGN KEY REFERENCES Marka(Markaid),
    Model         NVARCHAR(50),                     -- model elle yazilir
    Stok          INT NOT NULL DEFAULT 0,
    EkOzellikler  NVARCHAR(250)
);
GO

/* ---------- 7) MUSTERI ---------- */
CREATE TABLE Musteri (
    MusteriId         INT IDENTITY(1,1) PRIMARY KEY,
    TC                NVARCHAR(11),
    AdiSoyadi         NVARCHAR(100) NOT NULL,
    Mail              NVARCHAR(100),
    Telefon           NVARCHAR(15),
    Adres             NVARCHAR(250),
    UyelikTarihi      DATE NOT NULL DEFAULT GETDATE(),
    KayitEdenPersonel INT NOT NULL
                      CONSTRAINT FK_Musteri_Personel FOREIGN KEY REFERENCES Personel(PersonelNo)
);
GO

/* ---------- 8) ISLEM (durum listesi) ---------- */
CREATE TABLE Islem (
    IslemId  INT IDENTITY(1,1) PRIMARY KEY,
    Islem    NVARCHAR(50) NOT NULL
);
GO

/* ---------- 9) ARIZA KAYIT ---------- */
CREATE TABLE ArzaKayit (
    KayitId            INT IDENTITY(1,1) PRIMARY KEY,
    MusteriId          INT NOT NULL
                       CONSTRAINT FK_ArzaKayit_Musteri FOREIGN KEY REFERENCES Musteri(MusteriId),
    Cihaz              INT NOT NULL
                       CONSTRAINT FK_ArzaKayit_Cihaz FOREIGN KEY REFERENCES cihazlar(cihazid),
    Marka              INT NOT NULL
                       CONSTRAINT FK_ArzaKayit_Marka FOREIGN KEY REFERENCES Marka(Markaid),
    Model              NVARCHAR(50),                -- model elle yazilir
    Sikayet            NVARCHAR(500),
    TahminiArizaSebebi NVARCHAR(500),
    KaydiYapan         INT NOT NULL
                       CONSTRAINT FK_ArzaKayit_Personel FOREIGN KEY REFERENCES Personel(PersonelNo),
    KayitTarihi        DATETIME NOT NULL DEFAULT GETDATE(),
    Durumu             INT NOT NULL
                       CONSTRAINT FK_ArzaKayit_Islem FOREIGN KEY REFERENCES Islem(IslemId)
);
GO

/* ---------- 10) ARIZA DETAY ---------- */
CREATE TABLE ArzaDetay (
    DetayId      INT IDENTITY(1,1) PRIMARY KEY,
    Kayit        INT NOT NULL
                 CONSTRAINT FK_ArzaDetay_Kayit FOREIGN KEY REFERENCES ArzaKayit(KayitId),
    IslemTarihi  DATETIME NOT NULL DEFAULT GETDATE(),
    Islem        INT NOT NULL
                 CONSTRAINT FK_ArzaDetay_Islem FOREIGN KEY REFERENCES Islem(IslemId),
    IslemiYapan  INT NOT NULL
                 CONSTRAINT FK_ArzaDetay_Personel FOREIGN KEY REFERENCES Personel(PersonelNo),
    Tutar        DECIMAL(10,2) NOT NULL DEFAULT 0,
    Aciklama     NVARCHAR(500)
);
GO

/* ---------- 11) CARI ---------- */
CREATE TABLE Cari (
    CariId       INT IDENTITY(1,1) PRIMARY KEY,
    MusteriId    INT NOT NULL
                 CONSTRAINT FK_Cari_Musteri FOREIGN KEY REFERENCES Musteri(MusteriId),
    Tutar        DECIMAL(10,2) NOT NULL DEFAULT 0,
    KayitTarihi  DATETIME NOT NULL DEFAULT GETDATE(),
    Aktif        BIT NOT NULL DEFAULT 1             -- 1 aktif, 0 pasif (silmek yerine)
);
GO

/* ---------- 12) CARI DETAY ---------- */
CREATE TABLE CariDetay (
    Detayid               INT IDENTITY(1,1) PRIMARY KEY,
    MusteriId             INT NOT NULL
                          CONSTRAINT FK_CariDetay_Musteri FOREIGN KEY REFERENCES Musteri(MusteriId),
    IslemTutari           DECIMAL(10,2) NOT NULL DEFAULT 0,
    KalanBakiye           DECIMAL(10,2) NOT NULL DEFAULT 0,
    IslemDetay            NVARCHAR(500),
    IslemYapanKullanici   INT NOT NULL
                          CONSTRAINT FK_CariDetay_Personel FOREIGN KEY REFERENCES Personel(PersonelNo),
    IslemTarihi           DATETIME NOT NULL DEFAULT GETDATE()
);
GO

/* ---------- 13) SISTEM (hicbir tabloya baglanmaz) ---------- */
CREATE TABLE Sistem (
    AyarlarId           INT IDENTITY(1,1) PRIMARY KEY,
    SirketAdi           NVARCHAR(100),
    SirketVergiNo       NVARCHAR(20),
    SirketTelefon       NVARCHAR(15),
    SirketFax           NVARCHAR(15),
    SirketAdres         NVARCHAR(250),
    SirketMail          NVARCHAR(100),
    SirketGonderenMail  NVARCHAR(100),
    GonderenMailSifre   NVARCHAR(100)
);
GO

/* ============================================================
   BASLANGIC VERILERI (programi denerken lazim olacak)
   ============================================================ */

-- Islem (durum) listesi
INSERT INTO Islem (Islem) VALUES
(N'Giriş Yapıldı'),
(N'Tamirde'),
(N'Tamamlandı'),
(N'Tamamlanmadı');

-- Ornek cihaz turleri
INSERT INTO cihazlar (CihazAdi) VALUES
(N'Cep Telefonu'),
(N'Bilgisayar'),
(N'Tablet');

-- Tum yetkilere sahip admin yetkisi
INSERT INTO Yetkilendirme
    (YetkiAdi, PersonelEkleme, PersonelSilme, KayitAcma, KayitTamamlama, YedekParca, Carilislemleri, SistemAyarlari)
VALUES
    (N'Admin', 1, 1, 1, 1, 1, 1, 1);

-- Ilk giris icin admin personel (sifreyi sonra degistirin)
INSERT INTO Personel (TC, KullaniciAdi, AdiSoyadi, Gorevi, Yetkisi, Sifre)
VALUES (N'11111111111', N'admin', N'Admin', N'Yönetici', 1, N'1234');
GO

PRINT 'TeknikServis veritabani basariyla kuruldu.';
GO