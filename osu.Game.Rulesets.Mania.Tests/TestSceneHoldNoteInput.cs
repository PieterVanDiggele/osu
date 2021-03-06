// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using NUnit.Framework;
using osu.Framework.Screens;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Replays;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Mania.Objects;
using osu.Game.Rulesets.Mania.Replays;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;
using osu.Game.Screens.Play;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Mania.Tests
{
    public class TestSceneHoldNoteInput : RateAdjustedBeatmapTestScene
    {
        private const double time_before_head = 250;
        private const double time_head = 1500;
        private const double time_during_hold_1 = 2500;
        private const double time_tail = 4000;
        private const double time_after_tail = 5250;

        private List<JudgementResult> judgementResults;
        private bool allJudgedFired;

        /// <summary>
        ///     -----[           ]-----
        ///  o                           o
        /// </summary>
        [Test]
        public void TestNoInput()
        {
            performTest(new List<ReplayFrame>
            {
                new ManiaReplayFrame(time_before_head),
                new ManiaReplayFrame(time_after_tail),
            });

            assertHeadJudgement(HitResult.Miss);
            assertTickJudgement(HitResult.Miss);
            assertTailJudgement(HitResult.Miss);
            assertNoteJudgement(HitResult.Perfect);
        }

        /// <summary>
        ///     -----[           ]-----
        ///  x                           o
        /// </summary>
        [Test]
        public void TestPressTooEarlyAndReleaseAfterTail()
        {
            performTest(new List<ReplayFrame>
            {
                new ManiaReplayFrame(time_before_head, ManiaAction.Key1),
                new ManiaReplayFrame(time_after_tail, ManiaAction.Key1),
            });

            assertHeadJudgement(HitResult.Miss);
            assertTickJudgement(HitResult.Miss);
            assertTailJudgement(HitResult.Miss);
        }

        /// <summary>
        ///     -----[           ]-----
        ///  x                   o
        /// </summary>
        [Test]
        public void TestPressTooEarlyAndReleaseAtTail()
        {
            performTest(new List<ReplayFrame>
            {
                new ManiaReplayFrame(time_before_head, ManiaAction.Key1),
                new ManiaReplayFrame(time_tail),
            });

            assertHeadJudgement(HitResult.Miss);
            assertTickJudgement(HitResult.Miss);
            assertTailJudgement(HitResult.Miss);
        }

        /// <summary>
        ///     -----[           ]-----
        ///  xo      x                   o
        /// </summary>
        [Test]
        public void TestPressTooEarlyThenPressAtStartAndReleaseAfterTail()
        {
            performTest(new List<ReplayFrame>
            {
                new ManiaReplayFrame(time_before_head, ManiaAction.Key1),
                new ManiaReplayFrame(time_before_head + 10),
                new ManiaReplayFrame(time_head, ManiaAction.Key1),
                new ManiaReplayFrame(time_after_tail),
            });

            assertHeadJudgement(HitResult.Perfect);
            assertTickJudgement(HitResult.Perfect);
            assertTailJudgement(HitResult.Miss);
        }

        /// <summary>
        ///     -----[           ]-----
        ///  xo      x           o
        /// </summary>
        [Test]
        public void TestPressTooEarlyThenPressAtStartAndReleaseAtTail()
        {
            performTest(new List<ReplayFrame>
            {
                new ManiaReplayFrame(time_before_head, ManiaAction.Key1),
                new ManiaReplayFrame(time_before_head + 10),
                new ManiaReplayFrame(time_head, ManiaAction.Key1),
                new ManiaReplayFrame(time_tail),
            });

            assertHeadJudgement(HitResult.Perfect);
            assertTickJudgement(HitResult.Perfect);
            assertTailJudgement(HitResult.Perfect);
        }

        /// <summary>
        ///     -----[           ]-----
        ///          xo                  o
        /// </summary>
        [Test]
        public void TestPressAtStartAndBreak()
        {
            performTest(new List<ReplayFrame>
            {
                new ManiaReplayFrame(time_head, ManiaAction.Key1),
                new ManiaReplayFrame(time_head + 10),
                new ManiaReplayFrame(time_after_tail),
            });

            assertHeadJudgement(HitResult.Perfect);
            assertTickJudgement(HitResult.Miss);
            assertTailJudgement(HitResult.Miss);
        }

        /// <summary>
        ///     -----[           ]-----
        ///          xo  x               o
        /// </summary>
        [Test]
        public void TestPressAtStartThenBreakThenRepressAndReleaseAfterTail()
        {
            performTest(new List<ReplayFrame>
            {
                new ManiaReplayFrame(time_head, ManiaAction.Key1),
                new ManiaReplayFrame(time_head + 10),
                new ManiaReplayFrame(time_during_hold_1, ManiaAction.Key1),
                new ManiaReplayFrame(time_after_tail),
            });

            assertHeadJudgement(HitResult.Perfect);
            assertTickJudgement(HitResult.Perfect);
            assertTailJudgement(HitResult.Miss);
        }

        /// <summary>
        ///     -----[           ]-----
        ///          xo  x       o       o
        /// </summary>
        [Test]
        public void TestPressAtStartThenBreakThenRepressAndReleaseAtTail()
        {
            performTest(new List<ReplayFrame>
            {
                new ManiaReplayFrame(time_head, ManiaAction.Key1),
                new ManiaReplayFrame(time_head + 10),
                new ManiaReplayFrame(time_during_hold_1, ManiaAction.Key1),
                new ManiaReplayFrame(time_tail),
            });

            assertHeadJudgement(HitResult.Perfect);
            assertTickJudgement(HitResult.Perfect);
            assertTailJudgement(HitResult.Meh);
        }

        /// <summary>
        ///     -----[           ]-----
        ///             x                o
        /// </summary>
        [Test]
        public void TestPressDuringNoteAndReleaseAfterTail()
        {
            performTest(new List<ReplayFrame>
            {
                new ManiaReplayFrame(time_during_hold_1, ManiaAction.Key1),
                new ManiaReplayFrame(time_after_tail),
            });

            assertHeadJudgement(HitResult.Miss);
            assertTickJudgement(HitResult.Perfect);
            assertTailJudgement(HitResult.Miss);
        }

        /// <summary>
        ///     -----[           ]-----
        ///             x        o       o
        /// </summary>
        [Test]
        public void TestPressDuringNoteAndReleaseAtTail()
        {
            performTest(new List<ReplayFrame>
            {
                new ManiaReplayFrame(time_during_hold_1, ManiaAction.Key1),
                new ManiaReplayFrame(time_tail),
            });

            assertHeadJudgement(HitResult.Miss);
            assertTickJudgement(HitResult.Perfect);
            assertTailJudgement(HitResult.Meh);
        }

        /// <summary>
        ///     -----[           ]-----
        ///                      xo      o
        /// </summary>
        [Test]
        public void TestPressAndReleaseAtTail()
        {
            performTest(new List<ReplayFrame>
            {
                new ManiaReplayFrame(time_tail, ManiaAction.Key1),
                new ManiaReplayFrame(time_tail + 10),
            });

            assertHeadJudgement(HitResult.Miss);
            assertTickJudgement(HitResult.Miss);
            assertTailJudgement(HitResult.Meh);
        }

        private void assertHeadJudgement(HitResult result)
            => AddAssert($"head judged as {result}", () => judgementResults[0].Type == result);

        private void assertTailJudgement(HitResult result)
            => AddAssert($"tail judged as {result}", () => judgementResults[^2].Type == result);

        private void assertNoteJudgement(HitResult result)
            => AddAssert($"hold note judged as {result}", () => judgementResults[^1].Type == result);

        private void assertTickJudgement(HitResult result)
            => AddAssert($"tick judged as {result}", () => judgementResults[6].Type == result); // arbitrary tick

        private ScoreAccessibleReplayPlayer currentPlayer;

        private void performTest(List<ReplayFrame> frames)
        {
            AddStep("load player", () =>
            {
                Beatmap.Value = CreateWorkingBeatmap(new Beatmap<ManiaHitObject>
                {
                    HitObjects =
                    {
                        new HoldNote
                        {
                            StartTime = time_head,
                            Duration = time_tail - time_head,
                            Column = 0,
                        }
                    },
                    BeatmapInfo =
                    {
                        BaseDifficulty = new BeatmapDifficulty { SliderTickRate = 4 },
                        Ruleset = new ManiaRuleset().RulesetInfo
                    },
                });

                Beatmap.Value.Beatmap.ControlPointInfo.Add(0, new DifficultyControlPoint { SpeedMultiplier = 0.1f });

                var p = new ScoreAccessibleReplayPlayer(new Score { Replay = new Replay { Frames = frames } });

                p.OnLoadComplete += _ =>
                {
                    p.ScoreProcessor.NewJudgement += result =>
                    {
                        if (currentPlayer == p) judgementResults.Add(result);
                    };
                    p.ScoreProcessor.AllJudged += () =>
                    {
                        if (currentPlayer == p) allJudgedFired = true;
                    };
                };

                LoadScreen(currentPlayer = p);
                allJudgedFired = false;
                judgementResults = new List<JudgementResult>();
            });

            AddUntilStep("Beatmap at 0", () => Beatmap.Value.Track.CurrentTime == 0);
            AddUntilStep("Wait until player is loaded", () => currentPlayer.IsCurrentScreen());
            AddUntilStep("Wait for all judged", () => allJudgedFired);
        }

        private class ScoreAccessibleReplayPlayer : ReplayPlayer
        {
            public new ScoreProcessor ScoreProcessor => base.ScoreProcessor;

            protected override bool PauseOnFocusLost => false;

            public ScoreAccessibleReplayPlayer(Score score)
                : base(score, false, false)
            {
            }
        }
    }
}
