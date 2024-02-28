import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    scenarios: {
        stress: {
            executor: "per-vu-iterations",
            preAllocatedVUs: 200,
            iterations: 3,
            timeUnit: "1s",
            stages: [
                { duration: "2m", target: 10 },
                { duration: "5m", target: 10 },
                { duration: "2m", target: 20 },
                { duration: "5m", target: 20 },
                { duration: "2m", target: 30 },
                { duration: "5m", target: 30 },
                { duration: "2m", target: 40 },
                { duration: "5m", target: 40 },
                { duration: "10m", target: 0 },
            ],
        },
    },
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,
    headers: {
        'Content-Type': 'application/json',
        'Accept': '*/*'
    }
};

const domainName = "id.cdjb.uk";
const url = `https://${domainName}/register`;

export default function () {
    const payloads = [
        {
            emailAddress: `test${__VU}@test.com`,
            password: "dkfdfkjeifjeijeijffeifjefijeifjedkkd838392££$$$$!@;",
            phoneNumber: "01908231911"
        },
        {
            emailAddress: `test${__VU}@test.com`,
            password: "dkfdfkjeifjeijeijffeifjefijeifjedkkd838392££$$$$!@;",
            phoneNumber: "01908231911"
        },
        {
            emailAddress: `test${__VU}@test.com`,
            password: "dkfdfkjeifjeijeijffeifjefijeifjedkkd838392££$$$$!@;",
            phoneNumber: "01908231911"
        }
    ]

    const res = http.batch(
        payloads.map(payload => ({
            method: 'POST',
            url: url,
            body: JSON.stringify(payload),
            headers: options.headers,
        }))
    );

    for (let item in res) {
        check(item, { 'status was 201': (r) => r.status == 201 });
        sleep(1);
    }

    sleep(1);
}