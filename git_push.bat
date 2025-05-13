@echo off
cd /d %~dp0
echo ��ǰĿ¼��%cd%

echo.
set /p msg=�������ύ˵�������磺�޸�bug/�������ܣ���

rem === ��ȡϵͳ���� ===
for /f "tokens=1-3 delims=-/." %%a in ('echo %date%') do (
    set part1=%%a
    set part2=%%b
    set part3=%%c
)

rem === �ж���һ������� ===
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

rem === ȥ��ǰ���ո� ===
set yyyy=%yyyy: =%
set mm=%mm: =%
set dd=%dd: =%

rem === ���� ===
if 1%mm% LSS 110 set mm=0%mm%
if 1%dd% LSS 110 set dd=0%dd%

rem === ��ȡʱ�䲢��ʽ��Ϊ 24 Сʱ�� ===
for /f "tokens=1-2 delims=: " %%a in ('echo %time%') do (
    set hh=%%a
    set min=%%b
)

rem === ȥ��ǰ���ո� & ���� ===
set hh=%hh: =%
set min=%min: =%
if 1%hh% LSS 110 set hh=0%hh%
if 1%min% LSS 110 set min=0%min%

rem === ƴ��ʱ��� ===
set timestamp=%yyyy%-%mm%-%dd%_%hh%:%min%
echo ��ǰʱ���: %timestamp%

echo.
echo ��������ļ�...
git add .

echo.
echo �����ύ...
git commit -m "[%timestamp%] %msg%"

echo.
echo ������ȡԶ�̸���...
git pull --rebase origin master

echo.
echo �������͵� GitHub...
git push

echo.
echo ��ɣ���������˳�...
pause >nul
