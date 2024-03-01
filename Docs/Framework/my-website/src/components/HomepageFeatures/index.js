import clsx from 'clsx';
import Link from '@docusaurus/Link';
import Heading from '@theme/Heading';
import styles from './styles.module.css';

const FeatureList = [
  {
    title: 'Easy to Use',
    Svg: require('@site/static/img/undraw_docusaurus_mountain.svg').default,
    description: (
      <>
        A simple, flexible microservice framework.
      </>
    ),
  },
  {
    title: 'Focus on What Matters',
    Svg: require('@site/static/img/undraw_docusaurus_tree.svg').default,
    description: (
      <>
        Focus on deliverying value and not spending time writing boiler code!

      </>
    ),
  },
  {
    title: 'Powered by .NET',
    Svg: require('@site/static/img/undraw_docusaurus_react.svg').default,
    description: (
      <>
        Uses the latest version of .NET 8
      </>
    ),
  },
];

function Feature({ Svg, title, description }) {
  return (
    <div className={clsx('col col--4')}>

      <div className="text--center">
        <Svg className={styles.featureSvg} role="img" />
      </div>
      <div className="text--center padding-horiz--md">
        <Heading as="h3">{title}</Heading>
        <p>{description}</p>
      </div>
    </div>
  );
}

export default function HomepageFeatures() {
  return (
    <section className={styles.features}>
      <div className="container">
        <div className="row">
          
            <div className="padding-horiz--md">

              <p>Introducing MicroService Framework, the compact solution designed to enhance Microservice development with Vertical Slice Architecture in mind.</p>
              <p>Key Features:</p>
              <ol>
                <li><strong>Vertical Slice Architecture Support:</strong> Seamlessly integrate Vertical Slice Architecture into your Microservices, simplifying development and maintenance.</li>
                <li><strong>Built-in Health Check:</strong> Ensure the reliability of your Microservices with a built-in health check feature, enabling real-time monitoring of service status.</li>
                <li><strong>Idempotency:</strong> Guarantee consistency and reliability across operations with native idempotency support, minimizing unintended side effects.</li>
                <li><strong>Messaging (Rabbit MQ):</strong> Facilitate efficient communication and coordination between Microservices components with integrated messaging support via Rabbit MQ.</li>
                <li><strong>Security (JWT):</strong> Safeguard your Microservices with robust security measures, including JWT-based authentication, ensuring data integrity and confidentiality.</li>
                <li><strong>Structured Logging (Prometheus, Grafana, Blackbox), Error Handling, and Security:</strong> Maintain a resilient application ecosystem with structured logging through Prometheus, Grafana, and Blackbox, comprehensive error handling mechanisms, and fortified security protocols.</li>

              </ol>
              <h3 id='terminology'>Terminology:</h3>
              <ol>
                <li><strong>Feature:</strong> Represents a distinct vertical slice of functionality within the framework, encapsulating core logic.</li>
                <li><strong>Command:</strong> Execute server-side actions in alignment with the CQRS pattern, enabling efficient handling of write operations.</li>
                <li><strong>Query:</strong> Perform read-only actions integral to the CQRS pattern, facilitating data retrieval without altering the system state.</li>

              </ol>
              <p>(Note: A feature can only be designated as either a Command OR a Query, not both.)</p>
              <p>Elevate your Microservices architecture with MicroService Framework, empowering developers to build scalable, resilient, and secure applications with unparalleled ease and efficiency.Requirements</p>
              <ol>
                <li>NET 8</li>
                <li>Redis</li>
                <li>Prometheus</li>
                <li>Blackbox</li>
                <li>Grafana</li>
                <li>Rabbit MQ</li>

              </ol>
              <p>Though the above are requirements, you can swap Redis, Jeager and RabbitMQ to alternative platforms if required.</p>

              <Heading as="h2">Microservice Framework</Heading>
              <div className={styles.buttons}>
                <Link
                  className="button button--secondary button--lg"
                  to="/docs/category/setup">
                  Documentation
                </Link>
              </div>
            </div>

          {FeatureList.map((props, idx) => (
            <Feature key={idx} {...props} />
          ))}
        </div>
      </div>
    </section>
  );
}
