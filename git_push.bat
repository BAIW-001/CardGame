@echo off
cd /d %~dp0
echo ��ǰĿ¼��%cd%

echo.
set /p msg=�������ύ˵�������磺�޸�bug/�������ܣ���

if "%msg%"=="" (
    echo ? δ����˵����������ȡ����
    pause
    exit /b
)

echo.
echo ? ��������ļ�...
git add .

echo.
echo ? �����ύ����...
git commit -m "%msg%"

echo.
echo ? ������ȡԶ�̸���...
git pull --rebase origin master

echo.
echo ? �������͵� GitHub...
git push

echo.
echo ? ������ɣ���������˳�...
pause >nul
