/*  ───────────────────────────────────────────────────
 *  Secure Sequentier – Client-side i18n
 *  Supported: en, tr, es, de, fr
 *  ─────────────────────────────────────────────────── */

var I18N = (function () {

    /* ── supported languages ── */
    var LANGS = {
        en: { flag: '🇬🇧', name: 'English' },
        tr: { flag: '🇹🇷', name: 'Türkçe' },
        es: { flag: '🇪🇸', name: 'Español' },
        de: { flag: '🇩🇪', name: 'Deutsch' },
        fr: { flag: '🇫🇷', name: 'Français' }
    };

    /* ── translations dictionary ── */
    var T = {
        /* ─── NAVBAR ─── */
        'nav.signFiles':        { en:'Sign Files', tr:'Dosya İmzala', es:'Firmar Archivos', de:'Dateien Signieren', fr:'Signer des Fichiers' },
        'nav.verify':           { en:'Verify', tr:'Doğrula', es:'Verificar', de:'Verifizieren', fr:'Vérifier' },
        'nav.hash':             { en:'Hash Calculator', tr:'Hash Hesaplayıcı', es:'Calculadora Hash', de:'Hash-Rechner', fr:'Calculateur de Hash' },
        'nav.history':          { en:'History', tr:'Geçmiş', es:'Historial', de:'Verlauf', fr:'Historique' },
        'nav.about':            { en:'About', tr:'Hakkında', es:'Acerca de', de:'Über', fr:'À propos' },

        /* ─── INDEX / HERO ─── */
        'hero.title':           { en:'Secure Sequentier', tr:'Secure Sequentier', es:'Secure Sequentier', de:'Secure Sequentier', fr:'Secure Sequentier' },
        'hero.subtitle':        { en:'Sign, hash, and verify your files with SHA-256, SHA-512 & MD5.', tr:'SHA-256, SHA-512 ve MD5 ile dosyalarınızı imzalayın, hashleyin ve doğrulayın.', es:'Firme, hashee y verifique sus archivos con SHA-256, SHA-512 y MD5.', de:'Signieren, hashen und verifizieren Sie Ihre Dateien mit SHA-256, SHA-512 & MD5.', fr:'Signez, hachez et vérifiez vos fichiers avec SHA-256, SHA-512 et MD5.' },
        'hero.subtitle2':       { en:'Powered by real cryptographic algorithms.', tr:'Gerçek kriptografik algoritmalarla desteklenmektedir.', es:'Impulsado por algoritmos criptográficos reales.', de:'Unterstützt durch echte kryptografische Algorithmen.', fr:'Propulsé par de vrais algorithmes cryptographiques.' },
        'hero.signBtn':         { en:'Sign Files', tr:'Dosya İmzala', es:'Firmar Archivos', de:'Dateien Signieren', fr:'Signer des Fichiers' },
        'hero.verifyBtn':       { en:'Verify', tr:'Doğrula', es:'Verificar', de:'Verifizieren', fr:'Vérifier' },

        /* ─── STATS ─── */
        'stats.batches':        { en:'Total Batches', tr:'Toplam Grup', es:'Lotes Totales', de:'Gesamte Stapel', fr:'Lots Totaux' },
        'stats.files':          { en:'Files Processed', tr:'İşlenen Dosya', es:'Archivos Procesados', de:'Verarbeitete Dateien', fr:'Fichiers Traités' },
        'stats.signatures':     { en:'Signatures Created', tr:'Oluşturulan İmza', es:'Firmas Creadas', de:'Erstellte Signaturen', fr:'Signatures Créées' },
        'stats.users':          { en:'Active Users', tr:'Aktif Kullanıcı', es:'Usuarios Activos', de:'Aktive Benutzer', fr:'Utilisateurs Actifs' },

        /* ─── FEATURE CARDS ─── */
        'feat.signing':         { en:'Digital Signing', tr:'Dijital İmzalama', es:'Firma Digital', de:'Digitales Signieren', fr:'Signature Numérique' },
        'feat.signingDesc':     { en:'Upload your files and generate cryptographic signatures using SHA-256, SHA-512, and MD5 hash algorithms.', tr:'Dosyalarınızı yükleyin ve SHA-256, SHA-512 ve MD5 hash algoritmaları kullanarak kriptografik imzalar oluşturun.', es:'Suba sus archivos y genere firmas criptográficas usando algoritmos hash SHA-256, SHA-512 y MD5.', de:'Laden Sie Ihre Dateien hoch und erzeugen Sie kryptografische Signaturen mit SHA-256, SHA-512 und MD5.', fr:'Téléchargez vos fichiers et générez des signatures cryptographiques avec SHA-256, SHA-512 et MD5.' },
        'feat.verification':    { en:'File Verification', tr:'Dosya Doğrulama', es:'Verificación de Archivos', de:'Dateiverifikation', fr:'Vérification de Fichiers' },
        'feat.verificationDesc':{ en:'Verify file integrity by comparing a file against its known hash. Instantly confirm nothing was tampered with.', tr:'Dosya bütünlüğünü bilinen hash ile karşılaştırarak doğrulayın. Hiçbir müdahale yapılmadığını anında onaylayın.', es:'Verifique la integridad del archivo comparándolo con su hash conocido. Confirme al instante que nada fue manipulado.', de:'Überprüfen Sie die Dateiintegrität durch Vergleich mit dem bekannten Hash. Bestätigen Sie sofort, dass nichts manipuliert wurde.', fr:'Vérifiez l\'intégrité du fichier en le comparant à son hash connu. Confirmez instantanément qu\'il n\'a pas été altéré.' },
        'feat.hashCalc':        { en:'Hash Calculator', tr:'Hash Hesaplayıcı', es:'Calculadora Hash', de:'Hash-Rechner', fr:'Calculateur de Hash' },
        'feat.hashCalcDesc':    { en:'Calculate SHA-256, SHA-512, and MD5 hashes for any file instantly. Copy hashes with one click.', tr:'Herhangi bir dosya için SHA-256, SHA-512 ve MD5 hash değerlerini anında hesaplayın. Tek tıkla kopyalayın.', es:'Calcule hashes SHA-256, SHA-512 y MD5 para cualquier archivo al instante. Copie hashes con un clic.', de:'Berechnen Sie SHA-256, SHA-512 und MD5 Hashes für jede Datei sofort. Kopieren Sie Hashes mit einem Klick.', fr:'Calculez les hashes SHA-256, SHA-512 et MD5 de n\'importe quel fichier instantanément. Copiez en un clic.' },

        /* ─── UPLOAD SECTION ─── */
        'upload.title':         { en:'Start a Batch', tr:'Grup Başlat', es:'Iniciar un Lote', de:'Stapel Starten', fr:'Démarrer un Lot' },
        'upload.session':       { en:'Session', tr:'Oturum', es:'Sesión', de:'Sitzung', fr:'Session' },
        'upload.targetApp':     { en:'Target Signer App', tr:'Hedef İmzalama Uygulaması', es:'Aplicación de Firma Destino', de:'Ziel-Signierungsapp', fr:'Application de Signature Cible' },
        'upload.selectFiles':   { en:'Select Files', tr:'Dosya Seç', es:'Seleccionar Archivos', de:'Dateien Auswählen', fr:'Sélectionner des Fichiers' },
        'upload.dragDrop':      { en:'Drag & drop files here', tr:'Dosyaları buraya sürükleyin', es:'Arrastre y suelte archivos aquí', de:'Dateien hierher ziehen', fr:'Glissez-déposez les fichiers ici' },
        'upload.browse':        { en:'or click to browse', tr:'veya tıklayarak seçin', es:'o haga clic para buscar', de:'oder klicken zum Durchsuchen', fr:'ou cliquez pour parcourir' },
        'upload.signBtn':       { en:'Sign Files', tr:'Dosyaları İmzala', es:'Firmar Archivos', de:'Dateien Signieren', fr:'Signer les Fichiers' },
        'upload.largeWarn':     { en:'Large file(s) detected — processing may take longer', tr:'Büyük dosya(lar) tespit edildi — işlem daha uzun sürebilir', es:'Archivo(s) grande(s) detectado(s) — el procesamiento puede tardar más', de:'Große Datei(en) erkannt — Verarbeitung kann länger dauern', fr:'Fichier(s) volumineux détecté(s) — le traitement peut prendre plus de temps' },

        /* ─── QUEUE ─── */
        'queue.title':          { en:'Batch Status', tr:'Grup Durumu', es:'Estado del Lote', de:'Stapelstatus', fr:'État du Lot' },
        'queue.checking':       { en:'Checking status…', tr:'Durum kontrol ediliyor…', es:'Comprobando estado…', de:'Status wird überprüft…', fr:'Vérification du statut…' },
        'queue.loading':        { en:'Loading batch information…', tr:'Grup bilgisi yükleniyor…', es:'Cargando información del lote…', de:'Stapelinformationen werden geladen…', fr:'Chargement des informations du lot…' },
        'queue.download':       { en:'Download Results (ZIP)', tr:'Sonuçları İndir (ZIP)', es:'Descargar Resultados (ZIP)', de:'Ergebnisse Herunterladen (ZIP)', fr:'Télécharger les Résultats (ZIP)' },
        'queue.certificate':    { en:'View Certificate', tr:'Sertifikayı Görüntüle', es:'Ver Certificado', de:'Zertifikat Anzeigen', fr:'Voir le Certificat' },
        'queue.backUpload':     { en:'Back to Upload', tr:'Yüklemeye Dön', es:'Volver a Subir', de:'Zurück zum Upload', fr:'Retour au Téléchargement' },
        'queue.processing':     { en:'Processing…', tr:'İşleniyor…', es:'Procesando…', de:'Verarbeitung…', fr:'Traitement…' },
        'queue.failed':         { en:'Failed', tr:'Başarısız', es:'Fallido', de:'Fehlgeschlagen', fr:'Échoué' },
        'queue.complete':       { en:'Complete', tr:'Tamamlandı', es:'Completado', de:'Abgeschlossen', fr:'Terminé' },
        'queue.remaining':      { en:'remaining', tr:'kalan', es:'restante(s)', de:'verbleibend', fr:'restant(s)' },
        'queue.completed':      { en:'completed', tr:'tamamlandı', es:'completado(s)', de:'abgeschlossen', fr:'terminé(s)' },
        'queue.retries':        { en:'retries', tr:'tekrar deneme', es:'reintentos', de:'Wiederholungen', fr:'tentatives' },
        'queue.started':        { en:'Started', tr:'Başladı', es:'Iniciado', de:'Gestartet', fr:'Démarré' },
        'queue.finished':       { en:'Finished', tr:'Bitti', es:'Finalizado', de:'Beendet', fr:'Terminé' },

        /* ─── HISTORY ─── */
        'hist.title':           { en:'Run History', tr:'Çalıştırma Geçmişi', es:'Historial de Ejecución', de:'Ausführungsverlauf', fr:'Historique d\'Exécution' },
        'hist.date':            { en:'Date', tr:'Tarih', es:'Fecha', de:'Datum', fr:'Date' },
        'hist.runId':           { en:'Run ID', tr:'Çalıştırma ID', es:'ID de Ejecución', de:'Ausführungs-ID', fr:'ID d\'Exécution' },
        'hist.user':            { en:'User', tr:'Kullanıcı', es:'Usuario', de:'Benutzer', fr:'Utilisateur' },
        'hist.status':          { en:'Status', tr:'Durum', es:'Estado', de:'Status', fr:'Statut' },
        'hist.startedAt':       { en:'Started At', tr:'Başlangıç', es:'Iniciado en', de:'Gestartet um', fr:'Démarré à' },
        'hist.retries':         { en:'Retries', tr:'Tekrar', es:'Reintentos', de:'Wiederholungen', fr:'Tentatives' },
        'hist.liveStatus':      { en:'Live Status', tr:'Canlı Durum', es:'Estado en Vivo', de:'Livestatus', fr:'Statut en Direct' },
        'hist.details':         { en:'Details', tr:'Detaylar', es:'Detalles', de:'Details', fr:'Détails' },
        'hist.viewStatus':      { en:'View Status', tr:'Durumu Gör', es:'Ver Estado', de:'Status Anzeigen', fr:'Voir le Statut' },
        'hist.file':            { en:'File', tr:'Dosya', es:'Archivo', de:'Datei', fr:'Fichier' },
        'hist.finished':        { en:'Finished', tr:'Bitti', es:'Finalizado', de:'Beendet', fr:'Terminé' },
        'hist.first':           { en:'« First', tr:'« İlk', es:'« Primero', de:'« Erste', fr:'« Premier' },
        'hist.prev':            { en:'‹ Prev', tr:'‹ Önceki', es:'‹ Anterior', de:'‹ Zurück', fr:'‹ Précédent' },
        'hist.next':            { en:'Next ›', tr:'Sonraki ›', es:'Siguiente ›', de:'Weiter ›', fr:'Suivant ›' },
        'hist.last':            { en:'Last »', tr:'Son »', es:'Último »', de:'Letzte »', fr:'Dernier »' },

        /* ─── VERIFY ─── */
        'verify.title':         { en:'Verify File Integrity', tr:'Dosya Bütünlüğünü Doğrula', es:'Verificar Integridad del Archivo', de:'Dateiintegrität Verifizieren', fr:'Vérifier l\'Intégrité du Fichier' },
        'verify.desc':          { en:'Upload a file and provide a known hash to check if the file has been tampered with.', tr:'Bir dosya yükleyin ve dosyanın değiştirilip değiştirilmediğini kontrol etmek için bilinen bir hash girin.', es:'Suba un archivo y proporcione un hash conocido para verificar si el archivo ha sido manipulado.', de:'Laden Sie eine Datei hoch und geben Sie einen bekannten Hash ein, um zu prüfen ob die Datei manipuliert wurde.', fr:'Téléchargez un fichier et fournissez un hash connu pour vérifier si le fichier a été altéré.' },
        'verify.localNote':     { en:'Your file never leaves your browser', tr:'Dosyanız tarayıcınızdan asla çıkmaz', es:'Su archivo nunca sale de su navegador', de:'Ihre Datei verlässt niemals Ihren Browser', fr:'Votre fichier ne quitte jamais votre navigateur' },
        'verify.localNote2':    { en:'hashing is done locally.', tr:'hashleme yerel olarak yapılır.', es:'el hashing se realiza localmente.', de:'das Hashing wird lokal durchgeführt.', fr:'le hachage est effectué localement.' },
        'verify.fileLabel':     { en:'File to Verify', tr:'Doğrulanacak Dosya', es:'Archivo a Verificar', de:'Zu Verifizierende Datei', fr:'Fichier à Vérifier' },
        'verify.dragDrop':      { en:'Drag & drop file here', tr:'Dosyayı buraya sürükleyin', es:'Arrastre y suelte el archivo aquí', de:'Datei hierher ziehen', fr:'Glissez-déposez le fichier ici' },
        'verify.browse':        { en:'or click to browse', tr:'veya tıklayarak seçin', es:'o haga clic para buscar', de:'oder klicken zum Durchsuchen', fr:'ou cliquez pour parcourir' },
        'verify.algoLabel':     { en:'Hash Algorithm', tr:'Hash Algoritması', es:'Algoritmo Hash', de:'Hash-Algorithmus', fr:'Algorithme de Hash' },
        'verify.hashLabel':     { en:'Expected Hash', tr:'Beklenen Hash', es:'Hash Esperado', de:'Erwarteter Hash', fr:'Hash Attendu' },
        'verify.hashPlaceholder':{ en:'Paste the expected hash value here...', tr:'Beklenen hash değerini buraya yapıştırın...', es:'Pegue el valor hash esperado aquí...', de:'Fügen Sie den erwarteten Hash-Wert hier ein...', fr:'Collez la valeur de hash attendue ici...' },
        'verify.btn':           { en:'Verify', tr:'Doğrula', es:'Verificar', de:'Verifizieren', fr:'Vérifier' },
        'verify.hashing':       { en:'Hashing file locally...', tr:'Dosya yerel olarak hashleniyor...', es:'Calculando hash localmente...', de:'Datei wird lokal gehasht...', fr:'Hachage local du fichier...' },
        'verify.selectFile':    { en:'Please select a file.', tr:'Lütfen bir dosya seçin.', es:'Por favor seleccione un archivo.', de:'Bitte wählen Sie eine Datei.', fr:'Veuillez sélectionner un fichier.' },
        'verify.enterHash':     { en:'Please enter a hash value.', tr:'Lütfen bir hash değeri girin.', es:'Por favor ingrese un valor hash.', de:'Bitte geben Sie einen Hash-Wert ein.', fr:'Veuillez entrer une valeur de hash.' },
        'verify.match':         { en:'Match! File is authentic.', tr:'Eşleşti! Dosya orijinal.', es:'¡Coincide! El archivo es auténtico.', de:'Übereinstimmung! Datei ist authentisch.', fr:'Correspondance ! Le fichier est authentique.' },
        'verify.mismatch':      { en:'Mismatch! File may be tampered.', tr:'Eşleşmedi! Dosya değiştirilmiş olabilir.', es:'¡No coincide! El archivo puede estar manipulado.', de:'Keine Übereinstimmung! Datei könnte manipuliert sein.', fr:'Non-concordance ! Le fichier peut avoir été altéré.' },
        'verify.fileInfo':      { en:'File', tr:'Dosya', es:'Archivo', de:'Datei', fr:'Fichier' },
        'verify.algorithm':     { en:'Algorithm', tr:'Algoritma', es:'Algoritmo', de:'Algorithmus', fr:'Algorithme' },
        'verify.expected':      { en:'Expected', tr:'Beklenen', es:'Esperado', de:'Erwartet', fr:'Attendu' },
        'verify.actual':        { en:'Actual', tr:'Hesaplanan', es:'Actual', de:'Tatsächlich', fr:'Calculé' },
        'verify.hashError':     { en:'Error hashing file: ', tr:'Dosya hashleme hatası: ', es:'Error al calcular hash: ', de:'Fehler beim Hashen: ', fr:'Erreur de hachage : ' },

        /* ─── HASH CALCULATOR ─── */
        'hash.title':           { en:'Hash Calculator', tr:'Hash Hesaplayıcı', es:'Calculadora Hash', de:'Hash-Rechner', fr:'Calculateur de Hash' },
        'hash.desc':            { en:'Drop a file and instantly get its SHA-256, SHA-512, and MD5 hashes.', tr:'Bir dosya bırakın ve SHA-256, SHA-512 ve MD5 hash değerlerini anında alın.', es:'Suelte un archivo y obtenga instantáneamente sus hashes SHA-256, SHA-512 y MD5.', de:'Legen Sie eine Datei ab und erhalten Sie sofort deren SHA-256, SHA-512 und MD5 Hashes.', fr:'Déposez un fichier et obtenez instantanément ses hashes SHA-256, SHA-512 et MD5.' },
        'hash.localNote':       { en:'Your file never leaves your browser', tr:'Dosyanız tarayıcınızdan asla çıkmaz', es:'Su archivo nunca sale de su navegador', de:'Ihre Datei verlässt niemals Ihren Browser', fr:'Votre fichier ne quitte jamais votre navigateur' },
        'hash.localNote2':      { en:'all hashing is done locally.', tr:'tüm hashleme yerel olarak yapılır.', es:'todo el hashing se realiza localmente.', de:'das gesamte Hashing wird lokal durchgeführt.', fr:'tout le hachage est effectué localement.' },
        'hash.fileLabel':       { en:'Select File', tr:'Dosya Seç', es:'Seleccionar Archivo', de:'Datei Auswählen', fr:'Sélectionner un Fichier' },
        'hash.dragDrop':        { en:'Drag & drop file here', tr:'Dosyayı buraya sürükleyin', es:'Arrastre y suelte el archivo aquí', de:'Datei hierher ziehen', fr:'Glissez-déposez le fichier ici' },
        'hash.browse':          { en:'or click to browse', tr:'veya tıklayarak seçin', es:'o haga clic para buscar', de:'oder klicken zum Durchsuchen', fr:'ou cliquez pour parcourir' },
        'hash.calcBtn':         { en:'Calculate Hashes', tr:'Hash Hesapla', es:'Calcular Hashes', de:'Hashes Berechnen', fr:'Calculer les Hashes' },
        'hash.hashing':         { en:'Hashing file locally...', tr:'Dosya yerel olarak hashleniyor...', es:'Calculando hash localmente...', de:'Datei wird lokal gehasht...', fr:'Hachage local du fichier...' },
        'hash.selectFile':      { en:'Please select a file.', tr:'Lütfen bir dosya seçin.', es:'Por favor seleccione un archivo.', de:'Bitte wählen Sie eine Datei.', fr:'Veuillez sélectionner un fichier.' },
        'hash.copy':            { en:'Copy', tr:'Kopyala', es:'Copiar', de:'Kopieren', fr:'Copier' },
        'hash.copied':          { en:'Copied!', tr:'Kopyalandı!', es:'¡Copiado!', de:'Kopiert!', fr:'Copié !' },
        'hash.error':           { en:'Error hashing file: ', tr:'Dosya hashleme hatası: ', es:'Error al calcular hash: ', de:'Fehler beim Hashen: ', fr:'Erreur de hachage : ' },

        /* ─── ABOUT ─── */
        'about.title':          { en:'About Secure Sequentier', tr:'Secure Sequentier Hakkında', es:'Acerca de Secure Sequentier', de:'Über Secure Sequentier', fr:'À propos de Secure Sequentier' },
        'about.subtitle':       { en:'A web-based file signing and verification platform', tr:'Web tabanlı dosya imzalama ve doğrulama platformu', es:'Una plataforma web de firma y verificación de archivos', de:'Eine webbasierte Plattform zur Dateisignierung und -verifizierung', fr:'Une plateforme web de signature et vérification de fichiers' },
        'about.whatTitle':      { en:'What is Secure Sequentier?', tr:'Secure Sequentier Nedir?', es:'¿Qué es Secure Sequentier?', de:'Was ist Secure Sequentier?', fr:'Qu\'est-ce que Secure Sequentier ?' },
        'about.whatP1':         { en:'Secure Sequentier is a web-based application for <strong>digitally signing</strong>, <strong>hashing</strong>, and <strong>verifying</strong> files using industry-standard cryptographic algorithms. It processes files through configurable subsystem applications that generate signatures, hashes, and detailed reports.', tr:'Secure Sequentier, endüstri standardı kriptografik algoritmalar kullanarak dosyaları <strong>dijital olarak imzalamak</strong>, <strong>hashlemek</strong> ve <strong>doğrulamak</strong> için web tabanlı bir uygulamadır. Dosyaları imzalar, hashler ve detaylı raporlar oluşturan yapılandırılabilir alt sistem uygulamaları aracılığıyla işler.', es:'Secure Sequentier es una aplicación web para <strong>firmar digitalmente</strong>, <strong>hashear</strong> y <strong>verificar</strong> archivos usando algoritmos criptográficos estándar de la industria. Procesa archivos a través de aplicaciones de subsistemas configurables que generan firmas, hashes e informes detallados.', de:'Secure Sequentier ist eine webbasierte Anwendung zum <strong>digitalen Signieren</strong>, <strong>Hashen</strong> und <strong>Verifizieren</strong> von Dateien mit kryptografischen Standardalgorithmen. Es verarbeitet Dateien über konfigurierbare Subsystem-Anwendungen, die Signaturen, Hashes und detaillierte Berichte erzeugen.', fr:'Secure Sequentier est une application web pour <strong>signer numériquement</strong>, <strong>hacher</strong> et <strong>vérifier</strong> des fichiers en utilisant des algorithmes cryptographiques standards. Elle traite les fichiers via des applications de sous-systèmes configurables qui génèrent des signatures, des hashes et des rapports détaillés.' },
        'about.whatP2':         { en:'Built with <strong>ASP.NET Core</strong> on the backend and a modern MVC frontend, the application supports batch processing with real-time status updates via <strong>SignalR</strong>, automatic retry logic, configurable timeouts, and downloadable output packages.', tr:'Arka tarafta <strong>ASP.NET Core</strong> ve modern bir MVC ön yüzü ile inşa edilen uygulama, <strong>SignalR</strong> ile gerçek zamanlı durum güncellemeleri, otomatik yeniden deneme mantığı, yapılandırılabilir zaman aşımları ve indirilebilir çıktı paketleri ile toplu işlemeyi destekler.', es:'Construido con <strong>ASP.NET Core</strong> en el backend y un frontend MVC moderno, la aplicación soporta procesamiento por lotes con actualizaciones de estado en tiempo real vía <strong>SignalR</strong>, lógica de reintento automático, tiempos de espera configurables y paquetes de salida descargables.', de:'Entwickelt mit <strong>ASP.NET Core</strong> im Backend und einem modernen MVC-Frontend, unterstützt die Anwendung Stapelverarbeitung mit Echtzeit-Statusaktualisierungen über <strong>SignalR</strong>, automatische Wiederholungslogik, konfigurierbare Timeouts und herunterladbare Ausgabepakete.', fr:'Construit avec <strong>ASP.NET Core</strong> en backend et un frontend MVC moderne, l\'application prend en charge le traitement par lots avec des mises à jour d\'état en temps réel via <strong>SignalR</strong>, une logique de réessai automatique, des délais configurables et des packages de sortie téléchargeables.' },
        'about.howTitle':       { en:'How It Works', tr:'Nasıl Çalışır', es:'Cómo Funciona', de:'Wie es Funktioniert', fr:'Comment ça Marche' },
        'about.step1Title':     { en:'File Upload', tr:'Dosya Yükleme', es:'Carga de Archivos', de:'Datei-Upload', fr:'Téléchargement de Fichiers' },
        'about.step1Desc':      { en:'User uploads one or more files through the web interface. Files are sent to the backend API and stored in a <strong>watch directory</strong> organized by user and target app.', tr:'Kullanıcı web arayüzü üzerinden bir veya daha fazla dosya yükler. Dosyalar arka uç API\'ye gönderilir ve kullanıcı ve hedef uygulamaya göre düzenlenmiş bir <strong>izleme dizinine</strong> kaydedilir.', es:'El usuario sube uno o más archivos a través de la interfaz web. Los archivos se envían a la API del backend y se almacenan en un <strong>directorio de vigilancia</strong> organizado por usuario y aplicación destino.', de:'Der Benutzer lädt eine oder mehrere Dateien über die Weboberfläche hoch. Die Dateien werden an die Backend-API gesendet und in einem <strong>Überwachungsverzeichnis</strong> gespeichert, das nach Benutzer und Ziel-App organisiert ist.', fr:'L\'utilisateur télécharge un ou plusieurs fichiers via l\'interface web. Les fichiers sont envoyés à l\'API backend et stockés dans un <strong>répertoire de surveillance</strong> organisé par utilisateur et application cible.' },
        'about.step2Title':     { en:'Queue & Orchestration', tr:'Kuyruk ve Orkestrasyon', es:'Cola y Orquestación', de:'Warteschlange & Orchestrierung', fr:'File d\'attente & Orchestration' },
        'about.step2Desc':      { en:'The <strong>Orchestrator Background Service</strong> monitors the queue. When a job is pending, it dequeues it, spawns the configured subsystem executable, and passes input/output paths as arguments.', tr:'<strong>Orkestratör Arka Plan Servisi</strong> kuyruğu izler. Bir iş beklemedeyken, kuyruğundan çıkarır, yapılandırılmış alt sistem uygulamasını başlatır ve giriş/çıkış yollarını argüman olarak iletir.', es:'El <strong>Servicio de Fondo del Orquestador</strong> monitorea la cola. Cuando un trabajo está pendiente, lo desencola, ejecuta el subsistema configurado y pasa las rutas de entrada/salida como argumentos.', de:'Der <strong>Orchestrator-Hintergrunddienst</strong> überwacht die Warteschlange. Wenn ein Job ansteht, wird er aus der Warteschlange genommen, das konfigurierte Subsystem gestartet und die Ein-/Ausgabepfade als Argumente übergeben.', fr:'Le <strong>Service d\'Arrière-plan d\'Orchestration</strong> surveille la file d\'attente. Quand un travail est en attente, il le retire de la file, lance l\'exécutable du sous-système configuré et passe les chemins d\'entrée/sortie en arguments.' },
        'about.step3Title':     { en:'Subsystem Processing', tr:'Alt Sistem İşleme', es:'Procesamiento del Subsistema', de:'Subsystem-Verarbeitung', fr:'Traitement du Sous-système' },
        'about.step3Desc':      { en:'The signer apps read the input file, compute cryptographic hashes (SHA-256, SHA-512, MD5), and generate:', tr:'İmzalama uygulamaları giriş dosyasını okur, kriptografik hashler (SHA-256, SHA-512, MD5) hesaplar ve şunları oluşturur:', es:'Las aplicaciones de firma leen el archivo de entrada, calculan los hashes criptográficos (SHA-256, SHA-512, MD5) y generan:', de:'Die Signierungsanwendungen lesen die Eingabedatei, berechnen kryptografische Hashes (SHA-256, SHA-512, MD5) und erzeugen:', fr:'Les applications de signature lisent le fichier d\'entrée, calculent les hashes cryptographiques (SHA-256, SHA-512, MD5) et génèrent :' },
        'about.step3Sig':       { en:'<strong>Signature file</strong> (.sig.json) — structured signature with algorithm details', tr:'<strong>İmza dosyası</strong> (.sig.json) — algoritma detayları ile yapılandırılmış imza', es:'<strong>Archivo de firma</strong> (.sig.json) — firma estructurada con detalles del algoritmo', de:'<strong>Signaturdatei</strong> (.sig.json) — strukturierte Signatur mit Algorithmusdetails', fr:'<strong>Fichier de signature</strong> (.sig.json) — signature structurée avec détails de l\'algorithme' },
        'about.step3Hash':      { en:'<strong>Hash file</strong> (.hash.txt) — plain-text hash values', tr:'<strong>Hash dosyası</strong> (.hash.txt) — düz metin hash değerleri', es:'<strong>Archivo hash</strong> (.hash.txt) — valores hash en texto plano', de:'<strong>Hash-Datei</strong> (.hash.txt) — Klartext-Hash-Werte', fr:'<strong>Fichier hash</strong> (.hash.txt) — valeurs de hash en texte brut' },
        'about.step3Signed':    { en:'<strong>Signed copy</strong> — the original file with appended hash marker', tr:'<strong>İmzalı kopya</strong> — hash işaretçisi eklenmiş orijinal dosya', es:'<strong>Copia firmada</strong> — el archivo original con marcador hash adjunto', de:'<strong>Signierte Kopie</strong> — die Originaldatei mit angehängtem Hash-Marker', fr:'<strong>Copie signée</strong> — le fichier original avec marqueur de hash ajouté' },
        'about.step3Report':    { en:'<strong>Report</strong> (.report.txt) — detailed processing report with timestamps', tr:'<strong>Rapor</strong> (.report.txt) — zaman damgalı detaylı işleme raporu', es:'<strong>Informe</strong> (.report.txt) — informe detallado de procesamiento con marcas de tiempo', de:'<strong>Bericht</strong> (.report.txt) — detaillierter Verarbeitungsbericht mit Zeitstempeln', fr:'<strong>Rapport</strong> (.report.txt) — rapport de traitement détaillé avec horodatages' },
        'about.step4Title':     { en:'Results & Download', tr:'Sonuçlar ve İndirme', es:'Resultados y Descarga', de:'Ergebnisse & Download', fr:'Résultats & Téléchargement' },
        'about.step4Desc':      { en:'Once processing completes, results are stored in the <strong>processed directory</strong>. Users can view real-time status, download all outputs as a ZIP, or view a signature certificate.', tr:'İşlem tamamlandığında, sonuçlar <strong>işlenmiş dizine</strong> kaydedilir. Kullanıcılar gerçek zamanlı durumu görüntüleyebilir, tüm çıktıları ZIP olarak indirebilir veya imza sertifikasını görüntüleyebilir.', es:'Una vez completado el procesamiento, los resultados se almacenan en el <strong>directorio de procesados</strong>. Los usuarios pueden ver el estado en tiempo real, descargar todas las salidas como ZIP o ver un certificado de firma.', de:'Sobald die Verarbeitung abgeschlossen ist, werden die Ergebnisse im <strong>Verarbeitungsverzeichnis</strong> gespeichert. Benutzer können den Echtzeitstatus einsehen, alle Ausgaben als ZIP herunterladen oder ein Signaturzertifikat anzeigen.', fr:'Une fois le traitement terminé, les résultats sont stockés dans le <strong>répertoire de traitement</strong>. Les utilisateurs peuvent voir l\'état en temps réel, télécharger toutes les sorties en ZIP ou voir un certificat de signature.' },
        'about.algoTitle':      { en:'Algorithms Used', tr:'Kullanılan Algoritmalar', es:'Algoritmos Utilizados', de:'Verwendete Algorithmen', fr:'Algorithmes Utilisés' },
        'about.sha256Desc':     { en:'256-bit hash from the SHA-2 family. Used in TLS, SSL, Bitcoin, and digital signatures. Produces a 64-character hex string.', tr:'SHA-2 ailesinden 256-bit hash. TLS, SSL, Bitcoin ve dijital imzalarda kullanılır. 64 karakterlik hex dizesi üretir.', es:'Hash de 256 bits de la familia SHA-2. Usado en TLS, SSL, Bitcoin y firmas digitales. Produce una cadena hexadecimal de 64 caracteres.', de:'256-Bit-Hash aus der SHA-2-Familie. Verwendet in TLS, SSL, Bitcoin und digitalen Signaturen. Erzeugt eine 64-Zeichen-Hex-Zeichenkette.', fr:'Hash de 256 bits de la famille SHA-2. Utilisé dans TLS, SSL, Bitcoin et les signatures numériques. Produit une chaîne hexadécimale de 64 caractères.' },
        'about.sha512Desc':     { en:'512-bit hash offering higher security. Produces a 128-character hex string. Often used for password hashing and integrity verification.', tr:'Daha yüksek güvenlik sunan 512-bit hash. 128 karakterlik hex dizesi üretir. Genellikle parola hashleme ve bütünlük doğrulamada kullanılır.', es:'Hash de 512 bits que ofrece mayor seguridad. Produce una cadena hexadecimal de 128 caracteres. Frecuentemente usado para hasheo de contraseñas y verificación de integridad.', de:'512-Bit-Hash mit höherer Sicherheit. Erzeugt eine 128-Zeichen-Hex-Zeichenkette. Wird häufig für Passwort-Hashing und Integritätsprüfung verwendet.', fr:'Hash de 512 bits offrant une sécurité supérieure. Produit une chaîne hexadécimale de 128 caractères. Souvent utilisé pour le hachage de mots de passe et la vérification d\'intégrité.' },
        'about.md5Desc':        { en:'128-bit hash for quick checksums. Produces a 32-character hex string. Not recommended for security, but widely used for file integrity checks.', tr:'Hızlı sağlama toplamları için 128-bit hash. 32 karakterlik hex dizesi üretir. Güvenlik için önerilmez, ancak dosya bütünlüğü kontrolleri için yaygın olarak kullanılır.', es:'Hash de 128 bits para checksums rápidos. Produce una cadena hexadecimal de 32 caracteres. No recomendado para seguridad, pero ampliamente usado para verificación de integridad de archivos.', de:'128-Bit-Hash für schnelle Prüfsummen. Erzeugt eine 32-Zeichen-Hex-Zeichenkette. Nicht für Sicherheit empfohlen, aber weit verbreitet für Dateiintegritätsprüfungen.', fr:'Hash de 128 bits pour des sommes de contrôle rapides. Produit une chaîne hexadécimale de 32 caractères. Non recommandé pour la sécurité, mais largement utilisé pour les vérifications d\'intégrité de fichiers.' },
        'about.techTitle':      { en:'Technology Stack', tr:'Teknoloji Yığını', es:'Pila Tecnológica', de:'Technologie-Stack', fr:'Stack Technologique' },
        'about.author':         { en:'Developed as an internship project. Deployed on a home server via Docker and Cloudflare Tunnel.', tr:'Staj projesi olarak geliştirildi. Docker ve Cloudflare Tunnel ile ev sunucusuna dağıtıldı.', es:'Desarrollado como proyecto de prácticas. Desplegado en un servidor doméstico vía Docker y Cloudflare Tunnel.', de:'Entwickelt als Praktikumsprojekt. Auf einem Heimserver über Docker und Cloudflare Tunnel bereitgestellt.', fr:'Développé comme projet de stage. Déployé sur un serveur domestique via Docker et Cloudflare Tunnel.' },

        /* ─── CERTIFICATE ─── */
        'cert.title':           { en:'Signature Certificate', tr:'İmza Sertifikası', es:'Certificado de Firma', de:'Signaturzertifikat', fr:'Certificat de Signature' },
        'cert.printDesc':       { en:'Print this page or save as PDF for a formal record of the digital signature.', tr:'Dijital imzanın resmi kaydı için bu sayfayı yazdırın veya PDF olarak kaydedin.', es:'Imprima esta página o guárdela como PDF para un registro formal de la firma digital.', de:'Drucken Sie diese Seite oder speichern Sie sie als PDF für eine formale Aufzeichnung der digitalen Signatur.', fr:'Imprimez cette page ou enregistrez-la en PDF pour un enregistrement formel de la signature numérique.' },
        'cert.printBtn':        { en:'Print / Save as PDF', tr:'Yazdır / PDF Olarak Kaydet', es:'Imprimir / Guardar como PDF', de:'Drucken / Als PDF Speichern', fr:'Imprimer / Sauvegarder en PDF' },
        'cert.heading':         { en:'Certificate of Digital Signature', tr:'Dijital İmza Sertifikası', es:'Certificado de Firma Digital', de:'Zertifikat der Digitalen Signatur', fr:'Certificat de Signature Numérique' },
        'cert.certifies':       { en:'This certifies that the following file(s) have been digitally signed using the <strong>Secure Sequentier</strong> platform.', tr:'Bu, aşağıdaki dosya(ların) <strong>Secure Sequentier</strong> platformu kullanılarak dijital olarak imzalandığını onaylar.', es:'Esto certifica que el/los siguiente(s) archivo(s) han sido firmados digitalmente usando la plataforma <strong>Secure Sequentier</strong>.', de:'Hiermit wird bescheinigt, dass die folgenden Datei(en) mit der <strong>Secure Sequentier</strong>-Plattform digital signiert wurden.', fr:'Ceci certifie que le(s) fichier(s) suivant(s) ont été signés numériquement à l\'aide de la plateforme <strong>Secure Sequentier</strong>.' },
        'cert.batchId':         { en:'Batch ID', tr:'Grup ID', es:'ID del Lote', de:'Stapel-ID', fr:'ID du Lot' },
        'cert.userSession':     { en:'User Session', tr:'Kullanıcı Oturumu', es:'Sesión de Usuario', de:'Benutzersitzung', fr:'Session Utilisateur' },
        'cert.signerApp':       { en:'Signer App', tr:'İmzalama Uygulaması', es:'Aplicación de Firma', de:'Signierungsapp', fr:'Application de Signature' },
        'cert.dateIssued':      { en:'Date Issued', tr:'Düzenlenme Tarihi', es:'Fecha de Emisión', de:'Ausstellungsdatum', fr:'Date d\'Émission' },
        'cert.sigDetails':      { en:'Signature Details', tr:'İmza Detayları', es:'Detalles de la Firma', de:'Signaturdetails', fr:'Détails de la Signature' },
        'cert.generatedBy':     { en:'This certificate was generated by Secure Sequentier. Verify file integrity using the Verify tool at', tr:'Bu sertifika Secure Sequentier tarafından oluşturuldu. Dosya bütünlüğünü doğrulama aracını kullanarak doğrulayın:', es:'Este certificado fue generado por Secure Sequentier. Verifique la integridad del archivo usando la herramienta de verificación en', de:'Dieses Zertifikat wurde von Secure Sequentier erstellt. Überprüfen Sie die Dateiintegrität mit dem Verify-Tool unter', fr:'Ce certificat a été généré par Secure Sequentier. Vérifiez l\'intégrité du fichier avec l\'outil de vérification sur' },
        'cert.backBatch':       { en:'Back to Batch', tr:'Gruba Dön', es:'Volver al Lote', de:'Zurück zum Stapel', fr:'Retour au Lot' },

        /* ─── COMMON ─── */
        'common.file':          { en:'file(s)', tr:'dosya', es:'archivo(s)', de:'Datei(en)', fr:'fichier(s)' },
        'common.total':         { en:'total', tr:'toplam', es:'total', de:'gesamt', fr:'total' },
        'common.footer':        { en:'Secure Sequentier', tr:'Secure Sequentier', es:'Secure Sequentier', de:'Secure Sequentier', fr:'Secure Sequentier' }
    };

    /* ── detect language ── */
    function detect() {
        // 1) Check localStorage (user chose explicitly)
        var saved = localStorage.getItem('lang');
        if (saved && LANGS[saved]) return saved;

        // 2) Check navigator.language
        var nav = (navigator.language || navigator.userLanguage || 'en').toLowerCase();
        var prefix = nav.split('-')[0];
        if (LANGS[prefix]) return prefix;

        // 3) Default to English
        return 'en';
    }

    var currentLang = detect();

    /* ── translate helper ── */
    function t(key) {
        var entry = T[key];
        if (!entry) return key;
        return entry[currentLang] || entry['en'] || key;
    }

    /* ── translate all [data-i18n] elements ── */
    function applyAll() {
        var els = document.querySelectorAll('[data-i18n]');
        for (var i = 0; i < els.length; i++) {
            var el = els[i];
            var key = el.getAttribute('data-i18n');
            var entry = T[key];
            if (!entry) continue;
            var text = entry[currentLang] || entry['en'];
            // Check if translation contains HTML tags
            if (text && (text.indexOf('<') !== -1)) {
                el.innerHTML = text;
            } else {
                el.textContent = text;
            }
        }
        // Translate placeholders
        var phs = document.querySelectorAll('[data-i18n-placeholder]');
        for (var j = 0; j < phs.length; j++) {
            var ph = phs[j];
            var phKey = ph.getAttribute('data-i18n-placeholder');
            var phEntry = T[phKey];
            if (phEntry) {
                ph.placeholder = phEntry[currentLang] || phEntry['en'];
            }
        }
        // Translate title attributes
        var tts = document.querySelectorAll('[data-i18n-title]');
        for (var k = 0; k < tts.length; k++) {
            var tt = tts[k];
            var ttKey = tt.getAttribute('data-i18n-title');
            var ttEntry = T[ttKey];
            if (ttEntry) {
                tt.title = ttEntry[currentLang] || ttEntry['en'];
            }
        }
        // Update html lang attribute
        document.documentElement.lang = currentLang;
    }

    /* ── set language ── */
    function setLang(lang) {
        if (!LANGS[lang]) return;
        currentLang = lang;
        localStorage.setItem('lang', lang);
        applyAll();
    }

    /* ── get current ── */
    function getLang() { return currentLang; }

    /* ── get supported languages ── */
    function getLangs() { return LANGS; }

    /* ── auto-apply on DOM ready ── */
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', applyAll);
    } else {
        applyAll();
    }

    /* ── public API ── */
    return {
        t: t,
        setLang: setLang,
        getLang: getLang,
        getLangs: getLangs,
        apply: applyAll
    };
})();

