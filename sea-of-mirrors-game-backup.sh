#!/bin/bash

# ====================================================================
# 《鏡之海》總宇宙 - Google Drive 雙重防禦自動備份腳本 (含日期歷史檔)
# ====================================================================
# 功能：
#   1. 自動將外接硬碟的純淨資產，同步至 Google Drive 最新狀態。
#   2. 同時在雲端獨立建立一個「當天日期」的歷史存檔資料夾，做到里程碑備份。
#   3. 徹底物理隔離所有 Library、Temp 等暫存毒瘤。

# 1. 精準定義來源與目的地路徑
SRC="/Volumes/SiohuhuDev/Projects/sea-of-mirrors-game"
DST_ROOT="$HOME/Library/CloudStorage/GoogleDrive-zhenlong8886@gmail.com/我的雲端硬碟/SiohuhuDev 專案備份"

# 自動獲取當天日期（格式例如：2026-06-18）
TODAY=$(date '+%Y-%m-%d')

# 定義「最新同步」與「當天歷史備份」的具體雲端路徑
DST_LATEST="$DST_ROOT/最新同步_Latest"
DST_HISTORY="$DST_ROOT/歷史備份_History/${TODAY}_鏡之海備份"

# 2. 顯示備份啟動資訊
echo "=================================================="
echo "🚀 開始執行《鏡之海》雙重安全防禦備份..."
echo "📅 備份時間: $(date '+%Y-%m-%d %H:%M:%S')"
echo "📁 來源路徑: $SRC"
echo "☁️ 最新同步目的地: $DST_LATEST"
echo "⏳ 歷史時空節點目的地: $DST_HISTORY"
echo "=================================================="

# 檢查來源資料夾是否存在
if [ ! -d "$SRC" ]; then
  echo "❌ 錯誤：找不到來源資料夾！請檢查外接硬碟是否已正確連接。"
  exit 1
fi

# 確保雲端的目標目錄結構存在
mkdir -p "$DST_LATEST"
mkdir -p "$DST_HISTORY"

# 3. 執行備份過濾盾牌（排除所有暫存毒瘤與垃圾）
EXCLUDE_FLAGS=(
  --exclude='*/Library/'
  --exclude='*/Temp/'
  --exclude='*/Obj/'
  --exclude='*/Build/'
  --exclude='*/Builds/'
  --exclude='*/.git/'
  --exclude='.DS_Store'
  --exclude='*.textClipping'
)

# 【防線一：更新雲端最新狀態】
echo "🔄 正在更新 [最新同步] 資料夾..."
rsync -avh "${EXCLUDE_FLAGS[@]}" "$SRC/" "$DST_LATEST/"

echo "--------------------------------------------------"

# 【防線二：複製一份到當天的日期歷史資料夾】
echo "📦 正在建立 [ $TODAY ] 歷史時空節點..."
rsync -avh "${EXCLUDE_FLAGS[@]}" "$SRC/" "$DST_HISTORY/"

# 4. 備份完成提示
echo "=================================================="
echo "✨ 雙重備份順利完成！"
echo "🌟 雲端現已保存最新同步狀態，並成功獨立留存一份 [ $TODAY ] 歷史檔案。"
echo "🔒 所有的 Library 暫存與系統垃圾皆已完美隔離，不佔用雲端空間！"
echo "=================================================="