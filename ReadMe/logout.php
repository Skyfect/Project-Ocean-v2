<?php
$servername = "localhost";
$username = "root";
$password = "";
$dbname = "UnityGameDB";

$conn = new mysqli($servername, $username, $password, $dbname);

if ($conn->connect_error) {
    die("Bağlantı hatası: " . $conn->connect_error);
}

$nickname = $_POST['myNickName'];

// Kullanıcının oturum durumunu güncelle
$stmt = $conn->prepare("UPDATE myUser SET isOnline = 0 WHERE myNickName = ?");
$stmt->bind_param("s", $nickname);

if ($stmt->execute()) {
    echo "Çıkış başarılı";
} else {
    echo "Hata: " . $stmt->error;
}

$stmt->close();
$conn->close();
?>
