using System;
using System.Collections.Specialized;
using System.Threading;
using System.Windows.Forms;
using Quartz;
using Quartz.Impl;

namespace MonPFE
{
    public class SyncTimeManager
    {
        private ISchedulerFactory _schedulerFac;
        private IScheduler _scheduler;
        private IJobDetail _jobDetail;
        private ITrigger _trigger;

        private IScheduler _scheduler2;
        private IJobDetail _jobDetail2;
        private ITrigger _trigger2;

        public async void Init(SyncEngine engine)
        {
            // construct a scheduler factory
            NameValueCollection props = new NameValueCollection
            {
                {"quartz.serializer.type", "binary"}
            };
            _schedulerFac = new StdSchedulerFactory(props);

            // get a scheduler
            _scheduler = await _schedulerFac.GetScheduler();
            _scheduler2 = await _schedulerFac.GetScheduler();

            //await _scheduler.Start();

            // define the job and tie it to SyncEngine class
            _jobDetail = JobBuilder.Create<SyncEngine>()
                .WithIdentity("testConnJob", "group1")
                .Build();

            _jobDetail2 = JobBuilder.Create<FormInterface>()
                .WithIdentity("SyncJob", "group2")
                .Build();
            // Trigger the job to run now, and then every 120 seconds
            _trigger = TriggerBuilder.Create()
                .WithIdentity("each2minTrigger", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(120)
                    .RepeatForever())
                .Build();
            /*
            _trigger2 = TriggerBuilder.Create()
                .WithIdentity("testTrigger", "group2")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(5)
                    .RepeatForever())
                .Build();
            */

            _trigger2 = TriggerBuilder.Create()
                .WithIdentity("trigger3", "group2")
                .StartNow()
                .WithCronSchedule("0 0/1 * 1/1 * ? *")
                .Build();

            //

            //_scheduler.Context.Put("connectivityState", connectivityState);
            _scheduler.Context.Put("engine", engine);
            //_scheduler.Context.Put("formInterface", formInterface);

            await _scheduler.ScheduleJob(_jobDetail, _trigger);
            await _scheduler2.ScheduleJob(_jobDetail2, _trigger2);

        }


        public void RescheduleFromCronExpr(string cronExpr)
        {
            _trigger2 = TriggerBuilder.Create()
                .WithIdentity("trigger3", "group2")
                .StartNow()
                .WithCronSchedule(cronExpr)
                .Build();


            _scheduler2.RescheduleJob(new TriggerKey("trigger3", "group2"), _trigger2);

            //_scheduler2.ScheduleJob(_jobDetail2, _trigger2);
        }

        


        public void StartScheduler(int i)
        {
            if (i == 1)
                _scheduler.Start();
            else if (i == 2)
                _scheduler2.Start();
        }

        public  void StopScheduler(int i)
        {
            if (i == 1)
                _scheduler.PauseJob(_jobDetail.Key);
            else if (i == 2)
                _scheduler2.PauseJob(_jobDetail2.Key);
        }
    }
}