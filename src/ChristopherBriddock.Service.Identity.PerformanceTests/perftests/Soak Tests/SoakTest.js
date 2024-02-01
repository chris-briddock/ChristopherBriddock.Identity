import http from 'k6/http';
import { check, sleep } from 'k6';
import dotenv from 'dotenv';

dotenv.config(); 

export const options = {
    stages: [
        { duration: '2m', target: 100 },
        { duration: '3h50m', target: 100 },
        { duration: '2m', target: 0 },
    ],
    headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
    },
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,
};

const payload = JSON.stringify({
    emailAddress: `test${__VU}@test.com`,
    password: process.env.TEST_PASSWORD,
    phoneNumber: "01908231911",
});

const domainName = "localhost";
const port = "7078";
const url = `https://${domainName}:${port}/register`;

export default () => {
    const res = http.post(url, payload, options);

    check(res, {
        'status is 201': (r) => r.status === 201,
    });


    sleep(1);
};
