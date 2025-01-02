<?php
$servername = "localhost";
$username = "root";
$password = "";
$dbname = "UnityGameDB";

// Bağlantıyı oluştur
$conn = new mysqli($servername, $username, $password, $dbname);

// Bağlantıyı kontrol et
if ($conn->connect_error) {
    die("Bağlantı hatası: " . $conn->connect_error);
}

// Verileri al
$nickname = $_POST['myNickName'];
$password = $_POST['myPassword'];

// Kullanıcı adının benzersiz olup olmadığını kontrol et
$stmt = $conn->prepare("SELECT myNickName FROM myUser WHERE myNickName = ?");
$stmt->bind_param("s", $nickname);
$stmt->execute();
$stmt->store_result();

if ($stmt->num_rows > 0) {
    // Kullanıcı adı zaten var
    echo "Bu kullanıcı adı zaten alınmış";
} else {
    // Kullanıcı adı benzersiz, kaydı ekle
    $stmt->close();

    // Şifreyi hash'le
    $hashedPassword = password_hash($password, PASSWORD_DEFAULT);

    // Kayıt eklemek için yeni bir prepared statement oluştur
    $stmt = $conn->prepare("INSERT INTO myUser (myNickName, myPassword) VALUES (?, ?)");
    $stmt->bind_param("ss", $nickname, $hashedPassword);

    if ($stmt->execute()) {
        echo "Kayıt başarılı";
    } else {
        echo "Hata: " . $stmt->error;
    }
}

$stmt->close();
$conn->close();
?>
