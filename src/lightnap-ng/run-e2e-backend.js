const { execSync } = require('child_process');
const path = require('path');

process.chdir(path.join(__dirname, '../LightNap.WebApi'));
execSync('dotnet run --launch-profile E2e', { stdio: 'inherit' });
