@echo off
cd /d %~dp0
echo ��ǰĿ¼��%cd%

echo.
set /p msg=�������ύ˵�������磺�޸�bug/�������ܣ���

rem ��ȡԭʼϵͳ������ʱ��
for /f "tokens=1-3 delims=-/ " %%a in ("%date%") do (
    set d1=%%a
    set d2=%%b
    set d3=%%c
)

rem �ж���һ������ݣ���λ����
if %d1% gtr 999 (
    set yyyy=%d1%
    set mm=%d2%
    set dd=%d3%
) else if %d3% gtr 999 (
    set yyyy=%d3%
    set mm=%d2%
    set dd=%d1%
) else (
    set yyyy=%d2%
    set mm=%d1%
    set dd=%d3%
)

rem ��ȡСʱ�ͷ��ӣ�24Сʱ�ƣ����� PM/AM ���⣩
for /f "tokens=1,2 delims=: " %%a in ("%time%") do (
    set hh=%%a
    set min=%%b
)

rem ��ʽ��ʱ�䣨���㣩
if 1%hh% LSS 110 set hh=0%hh%
if 1%mm% LSS 110 set mm=0%mm%
if 1%dd%  LSS 110 set dd=0%dd%
if 1%min% LSS 110 set min=0%min%

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
