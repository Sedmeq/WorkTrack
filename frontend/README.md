# Employee Management System - Frontend

Bu frontend tətbiqi, Employee Management System üçün tam funksional authentication sistemi ilə təchiz edilmişdir.

## Xüsusiyyətlər

### 🔐 Authentication Sistemi
- **Login**: Mövcud istifadəçilər üçün giriş səhifəsi
- **Register**: Yeni istifadəçilər üçün qeydiyyat səhifəsi
- **JWT Token**: Təhlükəsiz authentication üçün JWT token istifadəsi
- **Auto-login**: Token localStorage-də saxlanılır və avtomatik giriş
- **Logout**: Təhlükəsiz çıxış funksiyası

### 👥 İşçi İdarəetməsi
- İşçilərin siyahısını görüntüləmə
- Yeni işçi əlavə etmə
- İşçi silmə
- Responsive dizayn

## Quraşdırma

### Tələb olunan paketlər
```bash
npm install react-router-dom
```

### Backend API
Frontend `https://localhost:7139/api` ünvanındakı backend API ilə işləyir.

## İstifadə

### 1. Qeydiyyat
- "Register" düyməsinə klikləyin
- İstifadəçi adı, email və şifrə daxil edin
- Şifrəni təsdiqləyin
- "Register" düyməsinə basın

### 2. Giriş
- Email və şifrənizi daxil edin
- "Login" düyməsinə basın
- Uğurlu girişdən sonra Dashboard səhifəsinə yönləndiriləcəksiniz

### 3. Dashboard
- İşçilərin siyahısını görə bilərsiniz
- Yeni işçi əlavə edə bilərsiniz
- Mövcud işçiləri silə bilərsiniz
- Sağ yuxarı küncdəki "Logout" düyməsi ilə çıxa bilərsiniz

## Fayl Strukturu

```
src/
├── components/
│   ├── Authentication.js    # Login/Register keçid komponenti
│   ├── Login.js            # Giriş səhifəsi
│   ├── Register.js         # Qeydiyyat səhifəsi
│   ├── Dashboard.js        # Ana dashboard
│   ├── Auth.css           # Authentication stilləri
│   └── Dashboard.css      # Dashboard stilləri
├── contexts/
│   └── AuthContext.js     # Authentication context
├── App.js                 # Ana komponent
├── App.css               # Ana stillər
└── index.js              # Giriş nöqtəsi
```

## API Endpoint-ləri

### Authentication
- `POST /api/auth/register` - Yeni istifadəçi qeydiyyatı
- `POST /api/auth/login` - İstifadəçi girişi

### İşçilər
- `GET /api/employees` - Bütün işçiləri əldə etmək
- `POST /api/employees` - Yeni işçi əlavə etmək
- `DELETE /api/employees/{id}` - İşçi silmək

## Təhlükəsizlik

- JWT token-lar localStorage-də saxlanılır
- Bütün API sorğuları avtomatik olaraq Authorization header ilə göndərilir
- Logout zamanı token təmizlənir

## Responsive Dizayn

Tətbiq bütün cihazlarda (desktop, tablet, mobil) düzgün işləyir və responsive dizayna malikdir.

## Xəta İdarəetməsi

- Form validasiyası
- API xətalarının göstərilməsi
- Loading state-ləri
- İstifadəçi dostu xəta mesajları

## Gələcək Təkmilləşdirmələr

- Şifrə bərpası
- Profil redaktəsi
- İşçi redaktəsi
- Axtarış və filtrasiya
- Pagination
- Real-time bildirişlər
