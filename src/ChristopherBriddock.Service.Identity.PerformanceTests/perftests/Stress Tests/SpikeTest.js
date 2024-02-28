import http from 'k6/http';
import { check, sleep } from 'k6';


const domainName = "id.cdjb.uk";
const url = `https://${domainName}/register`;

export const options = {
    vus: 10,
    stages: [
        { duration: '30s', target: 50 },  
        { duration: '5m', target: 10000000 },  
        { duration: '30s', target: 0 },
    ],
    headers: {
        'Content-Type': 'application/json',
        'Accept': "*/*"
    },
};

const payload = JSON.stringify({
    emailAddress: `test${__VU}@test.com`,
    password: "dkfdfkjeifjeijeijffeifjefijeifjedkkd838392££$$$$!@;",
    phoneNumber: "01908231911",
});

export default () => {
    const res = http.post(url, payload, options);

    check(res, {
        'status is 201': (r) => r.status === 201,
    });

    sleep(1);
};