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
$password = $_POST['myPassword'];

// Kullanıcı adını kontrol et
$stmt = $conn->prepare("SELECT myPassword, isOnline FROM myUser WHERE myNickName = ?");
$stmt->bind_param("s", $nickname);
$stmt->execute();
$stmt->store_result();

if ($stmt->num_rows > 0) {
    $stmt->bind_result($hashedPassword, $isOnline);
    $stmt->fetch();

    // Kullanıcı zaten online mı?
    if ($isOnline) {
        echo "Hata: Kullanıcı zaten oyunda";
    } else {
        // Şifreyi doğrula
        if (password_verify($password, $hashedPassword)) {
            // Giriş başarılı, isOnline durumunu güncelle
            $stmt->close();

            $updateStmt = $conn->prepare("UPDATE myUser SET isOnline = 1 WHERE myNickName = ?");
            $updateStmt->bind_param("s", $nickname);
            if ($updateStmt->execute()) {
                echo "Giriş başarılı";
            } else {
                echo "Hata: " . $updateStmt->error;
            }
            $updateStmt->close();
        } else {
            echo "Geçersiz kullanıcı adı veya şifre";
        }
    }
} else {
    echo "Geçersiz kullanıcı adı veya şifre";
}

$stmt->close();
$conn->close();
?>
