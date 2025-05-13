@echo off
cd /d %~dp0
echo 当前目录：%cd%

echo.
set /p msg=请输入提交说明（例如：修复bug/新增功能）：

rem === 获取系统日期 ===
for /f "tokens=1-3 delims=-/." %%a in ('echo %date%') do (
    set part1=%%a
    set part2=%%b
    set part3=%%c
)

rem === 判断哪一项是年份 ===
if %part1% GTR 1900 (
    set yyyy=%part1%
    set mm=%part2%
    set dd=%part3%
) else if %part3% GTR 1900 (
    set yyyy=%part3%
    set mm=%part2%
    set dd=%part1%
) else (
    set yyyy=%part2%
    set mm=%part1%
    set dd=%part3%
)

rem === 去掉前导空格 ===
set yyyy=%yyyy: =%
set mm=%mm: =%
set dd=%dd: =%

rem === 补零 ===
if 1%mm% LSS 110 set mm=0%mm%
if 1%dd% LSS 110 set dd=0%dd%

rem === 获取时间并格式化为 24 小时制 ===
for /f "tokens=1-2 delims=: " %%a in ('echo %time%') do (
    set hh=%%a
    set min=%%b
)

rem === 去掉前导空格 & 补零 ===
set hh=%hh: =%
set min=%min: =%
if 1%hh% LSS 110 set hh=0%hh%
if 1%min% LSS 110 set min=0%min%

rem === 拼接时间戳 ===
set timestamp=%yyyy%-%mm%-%dd%_%hh%:%min%
echo 当前时间戳: %timestamp%

echo.
echo 正在添加文件...
git add .

echo.
echo 正在提交...
git commit -m "[%timestamp%] %msg%"

echo.
echo 正在拉取远程更新...
git pull --rebase origin master

echo.
echo 正在推送到 GitHub...
git push

echo.
echo 完成！按任意键退出...
pause >nul
