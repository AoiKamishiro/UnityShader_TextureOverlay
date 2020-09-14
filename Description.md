# TextureOverlay 項目説明
## Rendering Mode
* Opaque - 完全に不透過なオブジェクトの描画用。
* Cutout - 透過・不透過かが明確なオブジェクトの描画用。切り抜き表現向きです。
* Transparent - 透過オブジェクトの描画用。反射などの映り込みは透過しません。
## Main
* Textures - 視界ジャックで表示するテクスチャを指定します。
    * Main Texture - 一枚目のテクスチャを指定します。
* Overlay Target
    * Player View (VR) - VRプレイヤーの視界をジャックします。
    * Player View (VR) - デスクトッププレイヤーの視界をジャックします。
    * Screen Shot - スクリーンショットをジャックします。
    * Other - その他の Unity カメラをジャックします。
* Rendering Options - 描画全般に関するオプションの設定です。
    * ZTest - ZTest の設定をします。
    * Render Queue - 描画時の処理順を設定します。