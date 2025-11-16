export function createAccessToken(
    userId: string,
    userName: string,
    email: string,
    overrides = {},
) {
    const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }));
    const payload = btoa(
        JSON.stringify({
            sub: userId,
            email: email,
            exp: Math.floor(Date.now() / 1000) + 3600,
            'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier':
                userId,
            'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name':
                userName,
            'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress':
                email,
            ...overrides,
        }),
    );
    return `${header}.${payload}.signature`;
}

export function setupAuthMocks() {
    cy.intercept('GET', '**/api/identity/access-token', (req) => {
        const cookieValue =
            req.headers['cookie']
                ?.toString()
                .match(/refreshToken=([^;]+)/)?.[1] || '';
        req.reply({
            statusCode: 200,
            body: {
                type: 'Success',
                result: cookieValue,
            },
        });
    }).as('getAccessToken');

    cy.intercept('POST', '**/api/identity/login', (req) => {
        const { login, password } = req.body;

        let token = null;
        if (login === 'test@example.com' && password === 'testpassword') {
            token = createAccessToken('test-user-id', 'TestUser', login);
        } else if (
            login === 'admin@admin.com' &&
            password === 'A2m!nPassword'
        ) {
            token = createAccessToken('admin-user-id', 'AdminUser', login, {
                'http://schemas.microsoft.com/ws/2008/06/identity/claims/role':
                    ['Administrator'],
            });
        } else if (
            login === 'contenteditor@lightnap.sharplogic.com' &&
            password === 'contenteditorpassword'
        ) {
            token = createAccessToken(
                'contenteditor-user-id',
                'ContentEditorUser',
                login,
                {
                    'http://schemas.microsoft.com/ws/2008/06/identity/claims/role':
                        ['ContentEditor'],
                },
            );
        }

        if (token) {
            req.reply({
                statusCode: 200,
                body: {
                    type: 'Success',
                    result: { type: 'AccessToken', accessToken: token },
                },
                headers: {
                    'Set-Cookie': `refreshToken=${token}; HttpOnly; Secure; SameSite=None`,
                    'Access-Control-Allow-Origin': 'http://localhost:4200',
                    'Access-Control-Allow-Credentials': 'true',
                },
            });
        } else {
            req.reply({
                statusCode: 200,
                body: { type: 'Error', errorMessages: ['Bad login'] },
            });
        }
    }).as('loginRequest');

    cy.intercept('GET', '**/api/identity/logout', (req) => {
        req.reply({
            statusCode: 200,
            body: {
                type: 'Success',
            },
            headers: {
                'Set-Cookie':
                    'refreshToken=; Expires=Thu, 01 Jan 1970 00:00:00 GMT; Max-Age=0; HttpOnly; Secure; SameSite=None',
                'Access-Control-Allow-Origin': 'http://localhost:4200',
                'Access-Control-Allow-Credentials': 'true',
            },
        });
    }).as('logoutRequest');

    cy.intercept('GET', '**/api/users/me/settings', {
        statusCode: 200,
        body: {
            type: 'Success',
            result: [
                { key: 'BrowserSettings', value: '' },
                { key: 'PreferredLanguage', value: '' },
            ],
        },
    }).as('getUserSettings');

    cy.intercept('POST', '**/api/users/me/notifications', {
        statusCode: 200,
        body: {
            type: 'Success',
            result: {
                unreadCount: 0,
                data: [],
                pageNumber: 1,
                pageSize: 10,
                totalCount: 0,
                totalPages: 0,
            },
        },
    }).as('getUserNotifications');

    cy.intercept('GET', '**/api/content/published/*/*', {
        statusCode: 200,
        body: {
            type: 'Success',
            result: {
                visibility: 'Reader',
                content: {
                    content: 'This is mocked published content.',
                    format: 'Html',
                },
            },
        },
    }).as('getPublishedContent');

    cy.intercept('GET', '**/api/content/supported-languages', {
        statusCode: 200,
        body: {
            type: 'Success',
            result: [
                { id: 'en', name: 'English' },
                { id: 'es', name: 'Spanish' },
            ],
        },
    }).as('getSupportedLanguages');

    cy.intercept('GET', '**/api/users/me/profile', {
        statusCode: 200,
        body: {
            type: 'Success',
            result: {},
        },
    }).as('getProfile');
}
export function setupContentMocks() {
    cy.intercept('POST', '**/api/content/search', {
        statusCode: 200,
        body: {
            type: 'Success',
            result: {
                data: [
                    {
                        key: 'sample-page',
                        type: 'Page',
                        status: 'Published',
                        readAccess: 'Public',
                        createdDate: '2024-01-01T00:00:00Z',
                        lastModifiedDate: '2024-01-01T00:00:00Z',
                    },
                    {
                        key: 'another-page',
                        type: 'Page',
                        status: 'Draft',
                        readAccess: 'Explicit',
                        createdDate: '2024-01-02T00:00:00Z',
                        lastModifiedDate: '2024-01-02T00:00:00Z',
                    },
                ],
                pageNumber: 1,
                pageSize: 10,
                totalCount: 2,
                totalPages: 1,
            },
        },
    }).as('searchContent');

    cy.intercept('POST', '**/api/content', {
        statusCode: 200,
        body: {
            type: 'Success',
            result: {
                key: 'new-content',
                type: 'Page',
                status: 'Draft',
                readAccess: 'Explicit',
                createdDate: '2024-01-03T00:00:00Z',
                lastModifiedDate: '2024-01-03T00:00:00Z',
            },
        },
    }).as('createContent');
}

export function setupAdminMocks() {
    cy.intercept('POST', '**/api/users/search', {
        statusCode: 200,
        body: {
            type: 'Success',
            result: {
                data: [
                    {
                        id: 'user-1',
                        userName: 'admin',
                        email: 'admin@admin.com',
                        createdDate: '2024-01-01T00:00:00Z',
                        lastModifiedDate: '2024-01-01T00:00:00Z',
                        lockoutEnd: null,
                    },
                    {
                        id: 'user-2',
                        userName: 'contenteditor',
                        email: 'contenteditor@lightnap.sharplogic.com',
                        createdDate: '2024-01-02T00:00:00Z',
                        lastModifiedDate: '2024-01-02T00:00:00Z',
                        lockoutEnd: null,
                    },
                ],
                pageNumber: 1,
                pageSize: 10,
                totalCount: 2,
                totalPages: 1,
            },
        },
    }).as('getUsers');

    cy.intercept('GET', '**/api/users/roles', {
        statusCode: 200,
        body: {
            type: 'Success',
            result: [
                {
                    name: 'Administrator',
                    displayName: 'Administrator',
                    description: 'Full administrative access to the system',
                },
                {
                    name: 'ContentEditor',
                    displayName: 'Content Editor',
                    description: 'Can manage content in the system',
                },
            ],
        },
    }).as('getRoles');

    cy.intercept('POST', '**/api/users/claims/search', {
        statusCode: 200,
        body: {
            type: 'Success',
            result: {
                data: [
                    {
                        type: 'Content:Reader',
                        value: 'sample-page',
                    },
                    {
                        type: 'Content:Editor',
                        value: 'another-page',
                    },
                ],
                pageNumber: 1,
                pageSize: 10,
                totalCount: 2,
                totalPages: 1,
            },
        },
    }).as('getClaims');
}
