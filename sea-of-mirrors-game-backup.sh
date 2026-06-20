#!/bin/bash

# ====================================================================
# 《鏡之海》總宇宙 - Google Drive 雙重防禦自動備份腳本 (終極天羅地網版)
# ====================================================================

set -e
set -o pipefail

# 1. 精準定義來源路徑
SRC="/Volumes/SiohuhuDev/Projects/sea-of-mirrors-game"

# 動態尋找 Google Drive 雲端硬碟掛載路徑
GDRIVE_BASE=$(find "$HOME/Library/CloudStorage" -maxdepth 1 -name "GoogleDrive-*" 2>/dev/null | head -n 1)

if [ -z "$GDRIVE_BASE" ]; then
  echo "❌ 錯誤：找不到 Google Drive 雲端硬碟掛載路徑，請確認 Google Drive App 是否已登入並同步。"
  exit 1
fi

DST_ROOT="$GDRIVE_BASE/我的雲端硬碟/SiohuhuDev 專案備份"
TODAY=$(date '+%Y-%m-%d')

DST_LATEST="$DST_ROOT/最新同步_Latest"
DST_HISTORY_DIR="$DST_ROOT/歷史備份_History"
DST_HISTORY_ZIP="$DST_HISTORY_DIR/${TODAY}_鏡之海備份.zip"

KEEP_HISTORY_COUNT=20

echo "=================================================="
echo "🚀 開始執行《鏡之海》雙重安全防禦備份..."
echo "📅 備份時間: $(date '+%Y-%m-%d %H:%M:%S')"
echo "📁 來源路徑: $SRC"
echo "☁️ 最新同步目的地: $DST_LATEST"
echo "⏳ 歷史時空節點目的地: $DST_HISTORY_ZIP"
echo "=================================================="

if [ ! -d "$SRC" ]; then
  echo "❌ 錯誤：找不到來源資料夾！請檢查外接硬碟是否已正確連接。"
  exit 1
fi

# 安全閥：因為下方 rsync 使用 --delete，若來源資料夾異常地空
# （例如硬碟掛載到一半、權限問題），要先擋下來，避免雲端內容被誤刪
SRC_ITEM_COUNT=$(find "$SRC" -mindepth 1 -maxdepth 1 | wc -l | tr -d ' ')
MIN_EXPECTED_ITEMS=5

if [ "$SRC_ITEM_COUNT" -lt "$MIN_EXPECTED_ITEMS" ]; then
  echo "❌ 錯誤：來源資料夾內容異常地少（僅 $SRC_ITEM_COUNT 項，預期至少 $MIN_EXPECTED_ITEMS 項）。"
  echo "⚠️  可能是硬碟掛載不完整或讀取異常，已中止備份以避免 --delete 誤刪雲端資料。"
  exit 1
fi

mkdir -p "$DST_LATEST"
mkdir -p "$DST_HISTORY_DIR"

# 3. 執行備份過濾盾牌（修正：同時封鎖「最頂層」與「子目錄深層」的暫存毒瘤）
RSYNC_EXCLUDE_FLAGS=(
  --exclude='Library/'
  --exclude='*/Library/'
  --exclude='Temp/'
  --exclude='*/Temp/'
  --exclude='Obj/'
  --exclude='*/Obj/'
  --exclude='Build/'
  --exclude='*/Build/'
  --exclude='Builds/'
  --exclude='*/Builds/'
  --exclude='.git/'
  --exclude='*/.git/'
  --exclude='.DS_Store'
  --exclude='*.textClipping'
)

# zip 排除清單（單一 -x，後面接多個 pattern；新增規則只需加一行字串）
ZIP_EXCLUDE_PATTERNS=(
  '*/Library/*'
  'Library/*'
  '*/Temp/*'
  'Temp/*'
  '*/Obj/*'
  'Obj/*'
  '*/Build/*'
  'Build/*'
  '*/Builds/*'
  'Builds/*'
  '*/.git/*'
  '.git/*'
  '*.DS_Store'
  '*/.DS_Store'
  '*.textClipping'
)

# 【防線一：更新雲端最新狀態】
echo "🔄 正在更新 [最新同步] 資料夾..."
rsync -avh --delete "${RSYNC_EXCLUDE_FLAGS[@]}" "$SRC/" "$DST_LATEST/"

echo "--------------------------------------------------"

# 【防線二：壓縮一份 zip 到當天的日期歷史資料夾】
echo "📦 正在建立 [ $TODAY ] 歷史時空節點（zip 壓縮）..."

if [ -f "$DST_HISTORY_ZIP" ]; then
  echo "⚠️  今日歷史備份已存在，將覆蓋舊檔。"
  rm -f "$DST_HISTORY_ZIP"
fi

(
  cd "$(dirname "$SRC")"
  zip -rq "$DST_HISTORY_ZIP" "$(basename "$SRC")" -x "${ZIP_EXCLUDE_PATTERNS[@]}"
)

echo "✅ 歷史備份壓縮完成：$DST_HISTORY_ZIP"

# 【防線三：清理過舊的歷史備份】
echo "--------------------------------------------------"
echo "🧹 正在檢查並清理舊歷史備份（僅保留最近 $KEEP_HISTORY_COUNT 份）..."

HISTORY_COUNT=$(ls -1 "$DST_HISTORY_DIR"/*.zip 2>/dev/null | wc -l | tr -d ' ')

if [ "$HISTORY_COUNT" -gt "$KEEP_HISTORY_COUNT" ]; then
  ls -t "$DST_HISTORY_DIR"/*.zip | tail -n +$((KEEP_HISTORY_COUNT + 1)) | xargs rm -f
  echo "🗑️  已清理多餘的舊歷史備份。"
else
  echo "👍 目前歷史備份數量（$HISTORY_COUNT）未超過上限，無需清理。"
fi

# 4. 備份完成提示
echo "=================================================="
echo "✨ 雙重備份順利完成！"
echo "🌟 雲端現已保存最新同步狀態，並成功獨立留存一份 [ $TODAY ] 歷史 zip 檔案。"
echo "🔒 Library、Temp 等暫存垃圾已隔離；_RawAssets~ 原始母帶已隨備份保留。"
echo "=================================================="