import http from 'k6/http';
import { check, sleep } from 'k6';   

const endpoint = "register";
const domainName = "id.cdjb.uk";
const url = `https://${domainName}/${endpoint}`;

export const options = {
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,
    vus: 5000000,
    duration: '5s'
};

const headers = {
        'Content-Type': 'application/json',
        'Accept': "application/json"
    }

const payload = JSON.stringify({
    emailAddress: `test${__VU}@test.com`,
    password: "dkfdfkjeifjeijeijffeifjefijeifjedkkd838392££$$$$!@;",
    phoneNumber: "01908231911"
});

export default () => {
    const res = http.post(url, payload, { headers });

    check(res, {
        'status is 201': (r) => r.status === 201,
    });
    sleep(1);
}