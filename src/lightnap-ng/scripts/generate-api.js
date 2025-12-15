const { exec } = require('child_process');
const path = require('path');

// Set environment variable
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

const command = 'npx orval';

console.log('Generating API client...');
exec(command, (error, stdout, stderr) => {
  // Clear the environment variable
  delete process.env.NODE_TLS_REJECT_UNAUTHORIZED;

  if (error) {
    console.error(`Error: ${error.message}`);
    process.exit(1);
  }
  if (stderr) {
    console.error(`stderr: ${stderr}`);
  }
  console.log('API client generated successfully!');
  console.log(stdout);
});
