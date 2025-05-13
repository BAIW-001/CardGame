@echo off
cd /d %~dp0
echo 当前目录：%cd%

echo.
set /p msg=请输入提交说明（例如：修复bug/新增功能）：

if "%msg%"=="" (
    echo ? 未输入说明，操作已取消。
    pause
    exit /b
)

echo.
echo ? 正在添加文件...
git add .

echo.
echo ? 正在提交更改...
git commit -m "%msg%"

echo.
echo ? 正在拉取远程更新...
git pull --rebase origin master

echo.
echo ? 正在推送到 GitHub...
git push

echo.
echo ? 操作完成！按任意键退出...
pause >nul
