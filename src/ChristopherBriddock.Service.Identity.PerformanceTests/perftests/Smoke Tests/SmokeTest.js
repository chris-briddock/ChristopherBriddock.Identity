import http from 'k6/http';
import { check, sleep } from 'k6';

const domainName = "localhost";
const port = "7078";
const url = `https://${domainName}:${port}/register`;

export let options = {
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,
    vus: 10,
    duration: '5s'
};

const headers = {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
};

const payload = JSON.stringify({
    emailAddress: `test${__VU}@test.com`,
    password: "kUVCE7Cl1UojsK5e2IbVYdA5eDenbtpsxslvMcPv",
    phoneNumber: "01908231911",
});

export default () => {
    const res = http.post(url, payload, { headers });

    check(res, {
        'status is 201': (r) => r.status === 201,
    });


    sleep(1);
};